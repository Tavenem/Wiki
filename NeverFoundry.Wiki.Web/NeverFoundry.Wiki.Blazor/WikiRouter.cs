using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Blazor
{
    /// <summary>
    /// A replacement for the default <see cref="Router"/> component which treats all routes
    /// relative to the base URL and starting with "wiki/" (not case sensitive) as internal. This
    /// allows the wiki to function (since its pages are not registered as normal routes).
    /// </summary>
    public class WikiRouter : IComponent, IHandleAfterRender, IDisposable
    {
        private static readonly ReadOnlyDictionary<string, object?> _EmptyParametersDictionary
            = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());

        private readonly RenderFragment<WikiRouteData> _renderWikiViewDelegate;
        private readonly RenderFragment _renderWikiViewCoreDelegate;

        private RenderHandle _renderHandle;
        private string? _baseUri;
        private string? _locationAbsolute;
        private ILogger<WikiRouter>? _logger;
        private bool _navigationInterceptionEnabled;

        /// <summary>
        /// Gets or sets a collection of additional assemblies that should be searched for components
        /// that can match URIs.
        /// </summary>
        [Parameter] public IEnumerable<Assembly>? AdditionalAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the assembly that should be searched for components matching the URI.
        /// </summary>
        [Parameter] public Assembly? AppAssembly { get; set; }

        /// <summary>
        /// Gets or sets the content to display when a match is found for the requested route.
        /// </summary>
        [Parameter] public RenderFragment<RouteData>? Found { get; set; }

        /// <summary>
        /// Gets or sets the content to display when no match is found for the requested route.
        /// </summary>
        [Parameter] public RenderFragment? NotFound { get; set; }

        /// <summary>
        /// <para>
        /// An outer layout within which wiki content will be displayed.
        /// </para>
        /// <para>
        /// If specified, must implement <see cref="IComponent"/> and accept a parameter named <see
        /// cref="LayoutComponentBase.Body"/>.
        /// </para>
        /// </summary>
        [Parameter] public Type? OuterLayout { get; set; }

        /// <summary>
        /// <para>
        /// The relative, root path which hosts all wiki content (without leading or trailing slashes).
        /// </para>
        /// <para>
        /// By default this is "wiki", resulting in all routes like {baseUri}/wiki/{route} to be
        /// treated as wiki pages.
        /// </para>
        /// </summary>
        [Parameter] public string WikiPath { get; set; } = "wiki";

        /// <summary>
        /// The route followed to arrive at the intended page.
        /// </summary>
        [Parameter] public WikiRouteData WikiRouteData { get; set; }

        [NotNull, Inject] private protected ILoggerFactory? LoggerFactory { get; set; }

        [NotNull, Inject] private protected INavigationInterception? NavigationInterception { get; set; }

        [NotNull, Inject] private protected NavigationManager? NavigationManager { get; set; }

        private RouteTable? Routes { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiRouter"/>.
        /// </summary>
        public WikiRouter()
        {
            _renderWikiViewDelegate = wikiRouteData => builder => RenderWikiView(builder, wikiRouteData);
            _renderWikiViewCoreDelegate = RenderWikiViewCore;
        }

        /// <summary>
        /// Attaches the component to a <see cref="RenderHandle" />.
        /// </summary>
        /// <param name="renderHandle">A <see cref="RenderHandle" /> that allows the component to be rendered.</param>
        public void Attach(RenderHandle renderHandle)
        {
            _logger = LoggerFactory.CreateLogger<WikiRouter>();
            _renderHandle = renderHandle;
            _baseUri = NavigationManager.BaseUri;
            _locationAbsolute = NavigationManager.Uri;

            NavigationManager.LocationChanged += OnLocationChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose() => NavigationManager.LocationChanged -= OnLocationChanged;

        /// <summary>
        /// Sets parameters supplied by the component's parent in the render tree.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <see cref="Task" /> that completes when the component has finished updating
        /// and rendering itself.</returns>
        /// <remarks>
        /// The <see cref="IComponent.SetParametersAsync(ParameterView)" /> method should be passed
        /// the entire set of parameter values each time <see
        /// cref="IComponent.SetParametersAsync(ParameterView)" /> is called. It not required that
        /// the caller supply a parameter value for all parameters that are logically understood by
        /// the component.
        /// </remarks>
        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (AppAssembly is null)
            {
                throw new InvalidOperationException($"The {nameof(WikiRouter)} component requires a value for the parameter {nameof(AppAssembly)}.");
            }

            if (Found is null)
            {
                throw new InvalidOperationException($"The {nameof(WikiRouter)} component requires a value for the parameter {nameof(Found)}.");
            }

            // NotFound content is mandatory, because even though we could display a default message like "Not found",
            // it has to be specified explicitly so that it can also be wrapped in a specific layout
            if (NotFound is null)
            {
                throw new InvalidOperationException($"The {nameof(WikiRouter)} component requires a value for the parameter {nameof(NotFound)}.");
            }

            var assemblies = AdditionalAssemblies == null ? new[] { AppAssembly } : new[] { AppAssembly }.Concat(AdditionalAssemblies);
            Routes = RouteTableFactory.Create(assemblies);
            Refresh();
            return Task.CompletedTask;
        }

        Task IHandleAfterRender.OnAfterRenderAsync()
        {
            if (!_navigationInterceptionEnabled)
            {
                _navigationInterceptionEnabled = true;
                return NavigationInterception.EnableNavigationInterceptionAsync();
            }

            return Task.CompletedTask;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs args)
        {
            _locationAbsolute = args.Location;
            if (_renderHandle.IsInitialized && Routes != null)
            {
                Refresh(args.IsNavigationIntercepted);
            }
        }

        private void Refresh(bool isNavigationIntercepted = false)
        {
            var locationPath = NavigationManager.ToBaseRelativePath(_locationAbsolute);

            var queryIndex = locationPath.IndexOf('?');
            var query = queryIndex < 0 ? null : locationPath[queryIndex..];
            locationPath = queryIndex < 0 ? locationPath : locationPath[..queryIndex];

            var anchorIndex = locationPath.IndexOf('#');
            var anchor = anchorIndex < 0 ? null : locationPath[anchorIndex..];
            locationPath = anchorIndex < 0 ? locationPath : locationPath[..anchorIndex];

            var context = new RouteContext(locationPath);
            Routes?.Route(context);

            if (context.Handler != null)
            {
                if (!typeof(IComponent).IsAssignableFrom(context.Handler))
                {
                    throw new InvalidOperationException($"The type {context.Handler.FullName} does not implement {typeof(IComponent).FullName}.");
                }

                Log.NavigatingToComponent(_logger, context.Handler, locationPath, _baseUri);

                var routeData = new RouteData(
                    context.Handler,
                    context.Parameters ?? _EmptyParametersDictionary);
                _renderHandle.Render(Found!(routeData));
            }
            else if (locationPath.StartsWith($"{WikiPath}/", StringComparison.OrdinalIgnoreCase))
            {
                locationPath = locationPath.Substring(5);
                var wikiRouteData = new WikiRouteData(
                    context.Segments.Length == 0 ? string.Empty : context.Segments[0],
                    string.IsNullOrEmpty(anchor) ? null : Uri.UnescapeDataString(anchor),
                    query);

                Log.NavigatingToWiki(_logger, _locationAbsolute, locationPath, _baseUri);
                _renderHandle.Render(_renderWikiViewDelegate(wikiRouteData));
            }
            else if (!isNavigationIntercepted)
            {
                Log.DisplayingNotFound(_logger, locationPath, _baseUri);

                // We did not find a Component that matches the route.
                // Only show the NotFound content if the application developer programatically got us here i.e we did not
                // intercept the navigation. In all other cases, force a browser navigation since this could be non-Blazor content.
                _renderHandle.Render(NotFound);
            }
            else
            {
                Log.NavigatingToExternalUri(_logger, _locationAbsolute, locationPath, _baseUri);
                NavigationManager.NavigateTo(_locationAbsolute, forceLoad: true);
            }
        }

        private void RenderWikiView(RenderTreeBuilder builder, WikiRouteData wikiRouteData)
        {
            WikiRouteData = wikiRouteData;
            if (OuterLayout is null)
            {
                _renderWikiViewCoreDelegate(builder);
            }
            else
            {
                builder.OpenComponent<LayoutView>(0);
                builder.AddAttribute(1, nameof(LayoutView.Layout), OuterLayout);
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderWikiViewCoreDelegate);
                builder.CloseComponent();
            }
        }

        private void RenderWikiViewCore(RenderTreeBuilder builder)
        {
            builder.OpenComponent<WikiView>(0);
            builder.AddAttribute(1, nameof(WikiView.WikiRoute), WikiRouteData.Page + WikiRouteData.Anchor + WikiRouteData.QueryParams);
            builder.CloseComponent();
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, string?, Exception?> _DisplayingNotFound =
                LoggerMessage.Define<string, string?>(LogLevel.Debug, new EventId(1, "DisplayingNotFound"), $"Displaying {nameof(NotFound)} because path '{{Path}}' with base URI '{{BaseUri}}' does not match any component route");

            private static readonly Action<ILogger, Type, string, string?, Exception?> _NavigatingToComponent =
                LoggerMessage.Define<Type, string, string?>(LogLevel.Debug, new EventId(2, "NavigatingToComponent"), "Navigating to component {ComponentType} in response to path '{Path}' with base URI '{BaseUri}'");

            private static readonly Action<ILogger, string?, string, string?, Exception?> _NavigatingToExternalUri =
                LoggerMessage.Define<string?, string, string?>(LogLevel.Debug, new EventId(3, "NavigatingToExternalUri"), "Navigating to non-component URI '{ExternalUri}' in response to path '{Path}' with base URI '{BaseUri}'");

            private static readonly Action<ILogger, string?, string?, string?, Exception?> _NavigatingToWiki =
                LoggerMessage.Define<string?, string?, string?>(LogLevel.Debug, new EventId(3, "NavigatingToWiki"), "Navigating to non-component URI '{ExternalUri}' as a wiki page in response to path '{Path}' with base URI '{BaseUri}'");

            internal static void DisplayingNotFound(ILogger? logger, string path, string? baseUri)
            {
                if (!(logger is null))
                {
                    _DisplayingNotFound(logger, path, baseUri, null);
                }
            }

            internal static void NavigatingToComponent(ILogger? logger, Type componentType, string path, string? baseUri)
            {
                if (!(logger is null))
                {
                    _NavigatingToComponent(logger, componentType, path, baseUri, null);
                }
            }

            internal static void NavigatingToExternalUri(ILogger? logger, string? externalUri, string path, string? baseUri)
            {
                if (!(logger is null))
                {
                    _NavigatingToExternalUri(logger, externalUri, path, baseUri, null);
                }
            }

            internal static void NavigatingToWiki(ILogger? logger, string? externalUri, string? path, string? baseUri)
            {
                if (!(logger is null))
                {
                    _NavigatingToWiki(logger, externalUri, path, baseUri, null);
                }
            }
        }

        private abstract class RouteConstraint
        {
            private static readonly ConcurrentDictionary<string, RouteConstraint> _CachedConstraints
                = new ConcurrentDictionary<string, RouteConstraint>();

            public abstract bool Match(string pathSegment, out object? convertedValue);

            public static RouteConstraint Parse(string template, string segment, string constraint)
            {
                if (string.IsNullOrEmpty(constraint))
                {
                    throw new ArgumentException($"Malformed segment '{segment}' in route '{template}' contains an empty constraint.");
                }

                if (_CachedConstraints.TryGetValue(constraint, out var cachedInstance))
                {
                    return cachedInstance;
                }
                else
                {
                    var newInstance = CreateRouteConstraint(constraint);
                    if (newInstance != null)
                    {
                        return _CachedConstraints.GetOrAdd(constraint, newInstance);
                    }
                    else
                    {
                        throw new ArgumentException($"Unsupported constraint '{constraint}' in route '{template}'.");
                    }
                }
            }

            private static RouteConstraint? CreateRouteConstraint(string constraint)
            {
                return constraint switch
                {
                    "bool" => new TypeRouteConstraint<bool>(bool.TryParse),
                    "datetime" => new TypeRouteConstraint<DateTime>((string str, out DateTime result)
                        => DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)),
                    "decimal" => new TypeRouteConstraint<decimal>((string str, out decimal result)
                        => decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result)),
                    "double" => new TypeRouteConstraint<double>((string str, out double result)
                        => double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result)),
                    "float" => new TypeRouteConstraint<float>((string str, out float result)
                        => float.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result)),
                    "guid" => new TypeRouteConstraint<Guid>(Guid.TryParse),
                    "int" => new TypeRouteConstraint<int>((string str, out int result)
                        => int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)),
                    "long" => new TypeRouteConstraint<long>((string str, out long result)
                        => long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)),
                    _ => null,
                };
            }
        }

        private class RouteContext
        {
            private static readonly char[] _Separator = new[] { '/' };

            public RouteContext(string path)
            {
                Segments = path.Trim('/').Split(_Separator, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < Segments.Length; i++)
                {
                    Segments[i] = Uri.UnescapeDataString(Segments[i]);
                }
            }

            public string[] Segments { get; }

            public Type? Handler { get; set; }

            public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
        }

        [DebuggerDisplay("Handler = {Handler}, Template = {Template}")]
        private class RouteEntry
        {
            public RouteEntry(RouteTemplate template, Type handler, string[] unusedRouteParameterNames)
            {
                Template = template;
                UnusedRouteParameterNames = unusedRouteParameterNames;
                Handler = handler;
            }

            public RouteTemplate Template { get; }

            public string[] UnusedRouteParameterNames { get; }

            public Type Handler { get; }

            internal void Match(RouteContext context)
            {
                if (Template.Segments.Length != context.Segments.Length)
                {
                    return;
                }

                Dictionary<string, object?>? parameters = null;
                for (var i = 0; i < Template.Segments.Length; i++)
                {
                    var segment = Template.Segments[i];
                    var pathSegment = context.Segments[i];
                    if (!segment.Match(pathSegment, out var matchedParameterValue))
                    {
                        return;
                    }
                    else
                    {
                        if (segment.IsParameter)
                        {
                            parameters ??= new Dictionary<string, object?>(StringComparer.Ordinal);
                            parameters[segment.Value] = matchedParameterValue;
                        }
                    }
                }

                if (UnusedRouteParameterNames.Length > 0)
                {
                    parameters ??= new Dictionary<string, object?>(StringComparer.Ordinal);
                    for (var i = 0; i < UnusedRouteParameterNames.Length; i++)
                    {
                        parameters[UnusedRouteParameterNames[i]] = null;
                    }
                }

                context.Parameters = parameters;
                context.Handler = Handler;
            }
        }

        private class RouteTable
        {
            public RouteTable(RouteEntry[] routes) => Routes = routes;

            public RouteEntry[] Routes { get; }

            internal void Route(RouteContext routeContext)
            {
                for (var i = 0; i < Routes.Length; i++)
                {
                    Routes[i].Match(routeContext);
                    if (routeContext.Handler != null)
                    {
                        return;
                    }
                }
            }
        }

        private static class RouteTableFactory
        {
            private static readonly ConcurrentDictionary<Key, RouteTable> _Cache =
                new ConcurrentDictionary<Key, RouteTable>();
            public static readonly IComparer<RouteEntry> RoutePrecedence = Comparer<RouteEntry>.Create(RouteComparison);

            public static RouteTable Create(IEnumerable<Assembly> assemblies)
            {
                var key = new Key(assemblies.OrderBy(a => a.FullName).ToArray());
                if (_Cache.TryGetValue(key, out var resolvedComponents))
                {
                    return resolvedComponents;
                }

                var componentTypes = key.Assemblies.SelectMany(a => a.ExportedTypes.Where(t => typeof(IComponent).IsAssignableFrom(t)));
                var routeTable = Create(componentTypes);
                _Cache.TryAdd(key, routeTable);
                return routeTable;
            }

            internal static RouteTable Create(IEnumerable<Type> componentTypes)
            {
                var templatesByHandler = new Dictionary<Type, string[]>();
                foreach (var componentType in componentTypes)
                {
                    var routeAttributes = componentType.GetCustomAttributes<RouteAttribute>(inherit: false);

                    var templates = routeAttributes.Select(t => t.Template).ToArray();
                    templatesByHandler.Add(componentType, templates);
                }
                return Create(templatesByHandler);
            }

            internal static RouteTable Create(Dictionary<Type, string[]> templatesByHandler)
            {
                var routes = new List<RouteEntry>();
                foreach (var keyValuePair in templatesByHandler)
                {
                    var parsedTemplates = keyValuePair.Value.Select(v => TemplateParser.ParseTemplate(v)).ToArray();
                    var allRouteParameterNames = parsedTemplates
                        .SelectMany(GetParameterNames)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    foreach (var parsedTemplate in parsedTemplates)
                    {
                        var unusedRouteParameterNames = allRouteParameterNames
                            .Except(GetParameterNames(parsedTemplate), StringComparer.OrdinalIgnoreCase)
                            .ToArray();
                        var entry = new RouteEntry(parsedTemplate, keyValuePair.Key, unusedRouteParameterNames);
                        routes.Add(entry);
                    }
                }

                return new RouteTable(routes.OrderBy(id => id, RoutePrecedence).ToArray());
            }

            private static string[] GetParameterNames(RouteTemplate routeTemplate)
            {
                return routeTemplate.Segments
                    .Where(s => s.IsParameter)
                    .Select(s => s.Value)
                    .ToArray();
            }

            internal static int RouteComparison(RouteEntry x, RouteEntry y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                var xTemplate = x.Template;
                var yTemplate = y.Template;
                if (xTemplate.Segments.Length != y.Template.Segments.Length)
                {
                    return xTemplate.Segments.Length < y.Template.Segments.Length ? -1 : 1;
                }
                else
                {
                    for (var i = 0; i < xTemplate.Segments.Length; i++)
                    {
                        var xSegment = xTemplate.Segments[i];
                        var ySegment = yTemplate.Segments[i];
                        if (!xSegment.IsParameter && ySegment.IsParameter)
                        {
                            return -1;
                        }
                        if (xSegment.IsParameter && !ySegment.IsParameter)
                        {
                            return 1;
                        }

                        if (xSegment.IsParameter)
                        {
                            if (xSegment.Constraints.Length > ySegment.Constraints.Length)
                            {
                                return -1;
                            }
                            else if (xSegment.Constraints.Length < ySegment.Constraints.Length)
                            {
                                return 1;
                            }
                        }
                        else
                        {
                            var comparison = string.Compare(xSegment.Value, ySegment.Value, StringComparison.OrdinalIgnoreCase);
                            if (comparison != 0)
                            {
                                return comparison;
                            }
                        }
                    }

                    throw new InvalidOperationException($@"The following routes are ambiguous:
'{x.Template.TemplateText}' in '{x.Handler.FullName}'
'{y.Template.TemplateText}' in '{y.Handler.FullName}'
");
                }
            }

            private readonly struct Key : IEquatable<Key>
            {
                public readonly Assembly[]? Assemblies;

                public Key(Assembly[]? assemblies) => Assemblies = assemblies;

                public override bool Equals(object obj) => obj is Key other && base.Equals(other);

                public bool Equals(Key other)
                {
                    if (Assemblies == null && other.Assemblies == null)
                    {
                        return true;
                    }
                    else if (Assemblies == null ^ other.Assemblies == null)
                    {
                        return false;
                    }
                    else if (Assemblies!.Length != other.Assemblies!.Length)
                    {
                        return false;
                    }

                    for (var i = 0; i < Assemblies.Length; i++)
                    {
                        if (!Assemblies[i].Equals(other.Assemblies[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                public override int GetHashCode()
                {
                    var hash = new HashCode();

                    if (Assemblies != null)
                    {
                        for (var i = 0; i < Assemblies.Length; i++)
                        {
                            hash.Add(Assemblies[i]);
                        }
                    }

                    return hash.ToHashCode();
                }
            }
        }

        [DebuggerDisplay("{TemplateText}")]
        private class RouteTemplate
        {
            public RouteTemplate(string templateText, TemplateSegment[] segments)
            {
                TemplateText = templateText;
                Segments = segments;
            }

            public string TemplateText { get; }

            public TemplateSegment[] Segments { get; }
        }

        private static class TemplateParser
        {
            public static readonly char[] InvalidParameterNameCharacters =
                new char[] { '*', '?', '{', '}', '=', '.' };

            internal static RouteTemplate ParseTemplate(string template)
            {
                var originalTemplate = template;
                template = template.Trim('/');
                if (template.Length == 0)
                {
                    return new RouteTemplate("/", Array.Empty<TemplateSegment>());
                }

                var segments = template.Split('/');
                var templateSegments = new TemplateSegment[segments.Length];
                for (var i = 0; i < segments.Length; i++)
                {
                    var segment = segments[i];
                    if (string.IsNullOrEmpty(segment))
                    {
                        throw new InvalidOperationException(
                            $"Invalid template '{template}'. Empty segments are not allowed.");
                    }

                    if (segment[0] != '{')
                    {
                        if (segment[^1] == '}')
                        {
                            throw new InvalidOperationException(
                                $"Invalid template '{template}'. Missing '{{' in parameter segment '{segment}'.");
                        }
                        templateSegments[i] = new TemplateSegment(originalTemplate, segment, isParameter: false);
                    }
                    else
                    {
                        if (segment[^1] != '}')
                        {
                            throw new InvalidOperationException(
                                $"Invalid template '{template}'. Missing '}}' in parameter segment '{segment}'.");
                        }

                        if (segment.Length < 3)
                        {
                            throw new InvalidOperationException(
                                $"Invalid template '{template}'. Empty parameter name in segment '{segment}' is not allowed.");
                        }

                        var invalidCharacter = segment.IndexOfAny(InvalidParameterNameCharacters, 1, segment.Length - 2);
                        if (invalidCharacter != -1)
                        {
                            throw new InvalidOperationException(
                                $"Invalid template '{template}'. The character '{segment[invalidCharacter]}' in parameter segment '{segment}' is not allowed.");
                        }

                        templateSegments[i] = new TemplateSegment(originalTemplate, segment[1..^1], isParameter: true);
                    }
                }

                for (var i = 0; i < templateSegments.Length; i++)
                {
                    var currentSegment = templateSegments[i];
                    if (!currentSegment.IsParameter)
                    {
                        continue;
                    }

                    for (var j = i + 1; j < templateSegments.Length; j++)
                    {
                        var nextSegment = templateSegments[j];
                        if (!nextSegment.IsParameter)
                        {
                            continue;
                        }

                        if (string.Equals(currentSegment.Value, nextSegment.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidOperationException(
                                $"Invalid template '{template}'. The parameter '{currentSegment}' appears multiple times.");
                        }
                    }
                }

                return new RouteTemplate(template, templateSegments);
            }
        }

        private class TemplateSegment
        {
            public TemplateSegment(string template, string segment, bool isParameter)
            {
                IsParameter = isParameter;

                if (!isParameter || segment.IndexOf(':') < 0)
                {
                    Value = segment;
                    Constraints = Array.Empty<RouteConstraint>();
                }
                else
                {
                    var tokens = segment.Split(':');
                    if (tokens[0].Length == 0)
                    {
                        throw new ArgumentException($"Malformed parameter '{segment}' in route '{template}' has no name before the constraints list.");
                    }

                    Value = tokens[0];
                    Constraints = tokens.Skip(1)
                        .Select(token => RouteConstraint.Parse(template, segment, token))
                        .ToArray();
                }
            }

            public string Value { get; }

            public bool IsParameter { get; }

            public RouteConstraint[] Constraints { get; }

            public bool Match(string pathSegment, out object? matchedParameterValue)
            {
                if (IsParameter)
                {
                    matchedParameterValue = pathSegment;

                    foreach (var constraint in Constraints)
                    {
                        if (!constraint.Match(pathSegment, out matchedParameterValue))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    matchedParameterValue = null;
                    return string.Equals(Value, pathSegment, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private class TypeRouteConstraint<T> : RouteConstraint
        {
            public delegate bool TryParseDelegate(string str, out T result);

            private readonly TryParseDelegate _parser;

            public TypeRouteConstraint(TryParseDelegate parser) => _parser = parser;

            public override bool Match(string pathSegment, out object? convertedValue)
            {
                if (_parser(pathSegment, out var result))
                {
                    convertedValue = result;
                    return true;
                }
                else
                {
                    convertedValue = null;
                    return false;
                }
            }
        }
    }
}
