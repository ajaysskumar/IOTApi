var gulp = require('gulp');

//Copy sb admin libraries to lib public/lib folder
gulp.task('copy', function () {
    gulp.src(['content/sb-admin-2/js/*.js'])
        .pipe(gulp.dest('public/lib/sbadmin/dist/js/'))
    //.pipe(gulp.dest('public/lib/sbadmin/dist/js/*.js'))

    gulp.src(['content/sb-admin-2/css/*.css'])
        .pipe(gulp.dest('public/lib/sbadmin/dist/css/'))

});
