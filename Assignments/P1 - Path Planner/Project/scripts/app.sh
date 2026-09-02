#!/usr/bin/env sh

# Find the executables we need
JAVA=`env which java`
FIND=`env which find`
ECHO=`env which echo`
HEAD=`env which head`

# Paths to JAR files, etc.
QTJ_DIR=./lib/io.qtjambi
QTJ_PAT=qtjambi-6.[0-9]*.[0-9]*.jar
PLANNER_JAR=PathPlannerApp.jar
SEARCH_JAR=./lib/uwu/student.jar
TILE_JAR=./lib/uwu/tiles.jar
DRIVER=edu.uwu.pathplanner.AppController

# Turn off newline trimming.
OLD_IFS=$IFS
IFS=

# Determine the qtjambi version(s); selects *earliest*.
QTJ_JAR="$($FIND $QTJ_DIR -type f -name $QTJ_PAT)"
QTJ_JAR=`$ECHO $QTJ_JAR | $HEAD -1`

# Run the project!
$JAVA -cp $QTJ_JAR:$TILE_JAR:$SEARCH_JAR:$PLANNER_JAR $DRIVER "$@"
