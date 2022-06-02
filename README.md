# MobileUndergradFinal
The mobile repository for my graduation project

It's idea is to be for water sources like waze is for potholes. It's not as good by a large margin.

It consumes an api hosted at: https://github.com/Vikcoc/BackendUndergradFinal

## Setup:
There are 2 files missing from git: AndroidManifestDebug.xml and settings-debug.json, both of them have a release counterpart in their respective folder that serves as a template.

For AndroidManifestDebug.xml in the line of code ```<meta-data android:name="com.google.android.maps.v2.API_KEY" android:value="KEY" />``` ```KEY``` must be replaced with a google maps sdk api key
https://developers.google.com/maps/documentation/android-sdk/overview

For settings-debug.json the url for the backend must be added
