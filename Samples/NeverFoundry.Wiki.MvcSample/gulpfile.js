"use strict";

const cssnano = require('cssnano');
const gulp = require('gulp');
const postcss = require('gulp-postcss');
const postcssPresetEnv = require('postcss-preset-env');
const sass = require('gulp-sass');
const sourcemaps = require('gulp-sourcemaps');

var paths = {
    styles: "./content/styles/site.scss",
    webroot: "./wwwroot/"
};

function compileStyles() {
    return gulp.src(paths.styles)
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(postcss([postcssPresetEnv(), cssnano()]))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(paths.webroot));
}

exports.default = gulp.parallel([
    compileStyles
]);
