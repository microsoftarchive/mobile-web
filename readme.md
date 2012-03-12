# Exploring the Mobile Web

## Mileage Stats Mobile

This is a work-in-progress reference application and part of [_Project Liike_](http://liike.github.com/).

Project Liike is an effort by the [Microsoft patterns & practices](http://msdn.microsoft.com/practices) team to produce guidance to help organizations build mobile web experiences based off of existing web applications. Its focus is on delivering HTML5 applications to a wide variety devices.

This reference application, which we call _Mileage Stats Mobile_, began with the source from a previous effort [Project Silk](http://silk.codeplex.com/). Project Silk was about building cross-browser web applications with a focus on client-side interactivity. However, it was optimized for desktop browser. Project Silk's reference application was simply called Mileage Stats. In the context of Project Liike we'll refer to it as the _legacy desktop experience_.

Project Liike is about extending and enhancing Mileage Stats with a mobile experience.

## Notes from the Journey

Mileage Stats Mobile is just one part of Project Liike. It's a journey and a learning experience for our team to build the application. We are recording our learning, insights, and frustrations along the way. We'll be publishing the distillation of these into our [docs repository](https://github.com/liike/docs).

We want your [feedback, comments, and questions](https://github.com/liike/reference-application/issues). Keep us honest.

## Project Status

_March 12, 2012_

We think of the application as providing three _experiences_.

The first is the _Legacy_ experience mentioned above. We deliver this if the server determines that the client browser is _not_ a mobile browser.

The second is the _Works_ experience. We deliver this as the standard baseline experience or when the server knows little about the client browser's capabilities. It is intended to work well on devices with limited resources (including network, CPU, screen size, etc.).

The third is the _Wow_ experience. We deliver this when it is determined that the client browser can support JavaScript, JSON, XHR, and DOM manipulation. The Wow and Works experiences are closely related however. Works is a subset of Wow, and generally follows a pattern of [Progressive Enhancement](http://en.wikipedia.org/wiki/Progressive_Enhancement). A key enhancement of Wow is providing the application as a [Single Page Application](http://en.wikipedia.org/wiki/Single_Page_Application) (SPA).

At this point, the Works experience is fairly mature whereas the Wow experience is only partially implemented. When running the application, the text "SPA" will appear in header when that mode is enabled.

## Running the Reference Application

The minimum requirement for building and running Mileage Stats Mobile is:

- Visual Studio 2010 Express with SP1
- ASP.NET MVC 3
- [Nuget 1.6+](https://nuget.org/)

The simplest way to get Visual Studio and MVC is to use the [Web Platform Installer](http://www.microsoft.com/web/downloads/platform.aspx).

Once everything is installed and you've downloaded the source, you'll need to make sure that all of the project's dependencies are present.

### Installing Dependencies

You have a couple of options.

- Open the solution in Visual Studio, and right-click on the Solution node in the Solution Explorer and select _Enable Package Restore_. This will configure Nuget to download the dependencies when building the solution. You can read more about this option in the [Nuget documentation](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages). 

- If you have the [Nuget CommandLine](http://www.nuget.org/packages/NuGet.CommandLine) installed, then you can use PowerShell to run the `install-packages.ps1` script in the root directory of the project.

## Project Principles

- This project values open, standards-based development
- We value the great work already available from others and intend to thank existing thought-leaders through attribution
- This work should happen in the open so everyone is able to follow along
- This project represents a conversation not a dictation
- We would like to meet the community where it is - using what it's using
- The guidance should be based on the best thinking available for the scenarios customers care about
- Will use the best open source libraries/frameworks available when appropriate