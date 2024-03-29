﻿namespace Tavenem.Wiki;

/// <summary>
/// A file in a category.
/// </summary>
/// <param name="Title">The title of the represented <see cref="WikiFile"/>.</param>
/// <param name="Size">The size of the file, in bytes.</param>
public record CategoryFile(PageTitle Title, int Size);
