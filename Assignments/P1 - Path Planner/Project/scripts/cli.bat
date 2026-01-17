@ECHO OFF

:: Paths to JAR files, etc.
set PLANNER_JAR=PathPlannerApp.jar
set SEARCH_JAR=.\lib\uwu\student.jar
set TILE_JAR=.\lib\uwu\tiles.jar
set DRIVER=edu.uwu.pathplanner.AppController

:: Find the Java runtime executable
set JAVA=
FOR /f %%r IN ('where java') DO set "JAVA=%%r" & GOTO :GOT_JAVA
:GOT_JAVA
IF NOT DEFINED JAVA (
echo "Error: Couldn't find Java!"
EXIT /b 1)

:: Run the project!
echo %JAVA% -cp %TILE_JAR%:%SEARCH_JAR%:%PLANNER_JAR% %DRIVER% %*
