"use strict";

const babelify = require('babelify');
const browserify = require('browserify');
const buffer = require('vinyl-buffer');
const concat = require('gulp-concat');
const cssnano = require('cssnano');
const gulp = require('gulp');
const postcss = require('gulp-postcss');
const postcssPresetEnv = require('postcss-preset-env');
const sass = require('gulp-sass');
const source = require('vinyl-source-stream');
const sourcemaps = require('gulp-sourcemaps');
const uglify = require('gulp-uglify');

var paths = {
    chat: "./content/chat.js",
    easymdecss: "./node_modules/easymde/dist/easymde.min.css",
    easymdejs: "./node_modules/easymde/dist/easymde.min.js",
    emojibuttonjs: "./node_modules/@joeattardi/emoji-button/dist/index.js",
    filepondcss: "./node_modules/filepond/dist/filepond.min.css",
    filepondjs: "./node_modules/filepond/dist/filepond.min.js",
    filepondpreviewcss: "./node_modules/filepond-plugin-image-preview/dist/filepond-plugin-image-preview.min.css",
    filepondpreviewjs: "./node_modules/filepond-plugin-image-preview/dist/filepond-plugin-image-preview.min.js",
    filepondsizejs: "./node_modules/filepond-plugin-file-validate-size/dist/filepond-plugin-file-validate-size.min.js",
    filepondtypejs: "./node_modules/filepond-plugin-file-validate-type/dist/filepond-plugin-file-validate-type.min.js",
    script: "./content/script.js",
    signalrjs: "./node_modules/@microsoft/signalr/dist/browser/signalr.min.js",
    styles: "./wwwroot/styles.scss",
    tippycss: "./node_modules/tippy.js/dist/tippy.css",
    webroot: "./wwwroot/",
};

function compileChat() {
    return browserify({
        entries: paths.chat,
        debug: true,
    }).transform(babelify, { presets: ['@babel/preset-env'], sourceMaps: true })
        .bundle()
        .pipe(source('chat.js'))
        .pipe(buffer())
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(uglify())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.webroot));
}

function compileScriptLibs() {
    return gulp.src([
            paths.easymdejs,
            paths.filepondjs,
            paths.filepondpreviewjs,
            paths.filepondsizejs,
            paths.filepondtypejs,
        ])
        .pipe(concat('libs.js'))
        .pipe(gulp.dest(paths.webroot));
}

function compileScripts() {
    return browserify({
        entries: paths.script,
        debug: true,
    }).transform(babelify, { presets: ['@babel/preset-env'], plugins: ['@babel/plugin-transform-runtime'], sourceMaps: true })
        .bundle()
        .pipe(source('script.js'))
        .pipe(buffer())
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(uglify())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.webroot));
}

function compileStyleLibs() {
    return gulp.src([paths.tippycss, paths.easymdecss, paths.filepondcss, paths.filepondpreviewcss])
        .pipe(concat('libstyles.css'))
        .pipe(gulp.dest(paths.webroot));
}

function compileStyles() {
    return gulp.src(paths.styles)
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(postcss([postcssPresetEnv(), cssnano()]))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.webroot));
}

exports.default = gulp.parallel([
    compileScriptLibs,
    compileScripts,
    compileChat,
    compileStyleLibs,
    compileStyles
]);
