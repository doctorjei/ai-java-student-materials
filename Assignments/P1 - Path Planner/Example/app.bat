@ECHO OFF

:: Paths to JAR files, etc.
set QTJ_DIR=.\lib\io.qtjambi
set QTJ_PAT=qtjambi-6.*.*.jar
set PLANNER_JAR=PathPlannerApp.jar
set DRIVER=edu.uwu.pathplanner.AppController

:: Find the Java runtime executable
set JAVA=
FOR /f %%r IN ('where java') DO set "JAVA=%%r" & GOTO :GOT_JAVA
:GOT_JAVA
IF NOT DEFINED JAVA (
echo "Error: Couldn't find Java!"
EXIT /b 1)

:: Find the QtJambi JAR(s); selects *earliest* version (first).
set QTJ_JAR=
FOR /f %%r IN ('dir %QTJ_DIR%\%QTJ_PAT% /b') DO set "QTJ_JAR=%%r" & GOTO :GOT_QTJ
:GOT_QTJ
IF NOT DEFINED QTJ_JAR (
echo "Error: Couldn't QtJambi!"
EXIT /b 1)

:: Run the project!
echo %JAVA% -cp %QTJ_DIR%\%QTJ_JAR%:%PLANNER_JAR% %DRIVER% %*
