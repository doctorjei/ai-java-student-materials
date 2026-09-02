cmake_minimum_required(VERSION 3.28)
find_package(Java 9 REQUIRED)
include(UseJava)

project(LinkedList)
set(SOURCE_FILES ListMain.java ../ai_structures/LinkedList.java)
add_jar(LinkedList ${SOURCE_FILES} ENTRY_POINT ListMain)
