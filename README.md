# IOSAndroidCameraFeed

Project that creates a view in native code for iOS and Android in order to support a live camera feed in .NET MAUI. 
The platform views are wrapped using the .NET MAUI handler concepts found [here](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/handlers/create).

The goal of the platform views is to enable common functionality
* Take Picture
* Switch from Front to Rear Cameras
* Take Video
* Capture a video frame (for machine vision projects or possible image filtering)
* Show live feed without forcing user to take photo or video
* Ask for permission automatically

This functionality is all without forcing UI on the user from the native platform views. The idea is that the CameraFeedView is placed in XAML or C# UI
and custom Buttons made by developer hook into the "API" of the CameraFeedView to use the functionality.
