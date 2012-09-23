# Exploring the Mobile Web

## Mileage Stats Mobile

This is a  reference application and part of [_Project Liike_](http://aka.ms/mobile-web) or __Building Modern Mobile Web Apps__.

Project Liike is an effort by the [Microsoft patterns & practices](http://msdn.microsoft.com/practices) team to produce guidance to help organizations build mobile web experiences based off of existing web applications. Its focus is on delivering HTML5 applications to a wide variety devices.

This reference application, which we call _Mileage Stats Mobile_, began with the source from a previous effort [Project Silk](http://silk.codeplex.com/). Project Silk was about building cross-browser web applications with a focus on client-side interactivity. However, it was optimized for desktop browser. Project Silk's reference application was simply called Mileage Stats. In the context of Project Liike we'll refer to it as the _legacy desktop experience_.

Project Liike is about extending and enhancing Mileage Stats with a mobile experience.

## Notes from the Journey

Mileage Stats Mobile is just one part of Project Liike. It's a journey and a learning experience for our team to build the application. We are recording our learning, insights, and frustrations along the way. We've be published the distillation of these onto [our MSDN site](http://aka.ms/mobile-web).

We want your feedback, comments, and questions. Keep us honest.

## Three Experiences

We think of the application as providing three _experiences_.

The first is the _Legacy_ experience mentioned above. We deliver this if the server determines that the client browser is _not_ a mobile browser.

The second is the _Works_ experience. We deliver this as the standard baseline experience or when the server knows little about the client browser's capabilities. It is intended to work well on devices with limited resources (including network, CPU, screen size, etc.).

The third is the _Wow_ experience. We deliver this when it is determined that the client browser can support JavaScript, JSON, XHR, and DOM manipulation. The Wow and Works experiences are closely related however. Works is a subset of Wow, and generally follows a pattern of [Progressive Enhancement](http://en.wikipedia.org/wiki/Progressive_Enhancement). A key enhancement of Wow is providing the application as a [Single Page Application](http://en.wikipedia.org/wiki/Single_Page_Application) (SPA).

When running the application, the text "SPA" will appear in header when that mode is enabled.

## Running the Reference Application

The minimum requirement for building and running Mileage Stats Mobile is:

- Visual Studio 2010 Express with SP1
- ASP.NET MVC 4
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

## Known Issues

- Issue #442 Unhandled exception in baseController getvehiclename for desktop experience. The reason because OnActionExecuted event when views are loaded we check for non null vehicleid. however filterContext.RouteData.Values["vehicleId"] returns zero for non existant vehicles then gevehicle name throws a no matching element. the fix is to check for non null and non zero

	 $exception	{"Sequence contains no matching element"}	System.Exception {System.InvalidOperationException}
	
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var vehicleId = filterContext.RouteData.Values["vehicleId"];
            if (vehicleId != null && string.IsNullOrEmpty(ViewBag.VehicleName))
            {
                ViewBag.VehicleName = GetVehicleName(int.Parse(vehicleId.ToString()));
            }
            base.OnActionExecuted(filterContext);
        }

- Issue #419 HTC(Status) does not give a pop up message to share location access.
Repo: 
	1. login to app. 
	2. Add a vehicle. 
	3. Add fill ups
Here click on "Locate near by fill-up station" . 
We don't get any pop up message to "share the locations" in these two devices.

- Issue #377 Successfully added fillup, reminder flash message shows on Dashboard,Fillup,chart page in Blackberry curve 9300 3G(RIM OS 5.0).
Repo:	
	1. Add a vehicle.
	2. Add a reminder.
	3. Go to Dashboard.
	 "Successfully added reminder shows up"
	4. Go to Fill up page
	 "Successfully added reminder shows up"
	5. Go to charts
	 "Successfully added reminder shows up"
	Its happening only in this work device.

- Issue #325 BlackBerry 9300(Curve 3G OS 5.0) Reminder Page shows more white space for added reminders.

- Issue #243 iOS 5 - Placeholders not going away when typing text instead of integers
Repro:
	1. Select Add Fillup page on iPhone 4S
	2. In the price per unit textbox, type "test", the placeholder "0" does not go away (it disappears when typing a number)

- Issue #179 Some Android phones status renders select list incorrectly. 	all controls with drop down are incorrectly rendered in this wow device

- Issue #156 validation summary markup.
Can we alter the output from the helper? We don't want to have the style inline.

        <div class="validation-summary-errors"><ul><li style="display:none"></li></ul></div>

- Issue #48 Alternate Authentication using ACS. I consider this a lower priority task, but I anticipate that the dev team will have extra time to fill.
Based on the last advisory board meeting, it seemed the majority of the advisers were not too concerned, however at least some were vocal about their interest. 
I would like to keep the main source free of dependencies as much as possible, so here are two possibilities:
	- maintain a branch on github that uses ACS, but allow the master branch to continue with the current auth 
	- have a pluggable auth system where we can change a config option to switch between the current auth and the ACS based auth

	The first option has the drawback that we'll have to periodically merge the master back in.
The second option would also need to try to partition the dependencies as much as possible. For example, perhaps the ACS-based auth provider could live in a separate repo.|