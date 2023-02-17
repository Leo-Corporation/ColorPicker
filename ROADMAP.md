# Roadmap for ColorPicker Max
![Banner](https://github.com/Leo-Corporation/LeoCorp-Docs/raw/master/Images/ColorPicker%20Max%20Banner.png)

## Introduction
ColorPicker Max, codename “Project Spectrum” is the next major iteration of Léo Corporation’s popular software ColorPicker. This new version will be written using the latest features of .NET and C#. It will be a complete rewrite of ColorPicker from the ground up. It will include a brand-new designed based on InternetTest Pro’s. It will also feature an integration with the newly launched Synethia algorithm library.

## Features
- [x]	Picker
    - [x]	Selector
        - [x]	Possibility to select a specific color using different sliders.
        - [x]	Possibility to select a color from a pixel on the screen.
        - [x]	Possibility to choose a color randomly.
        - [x]	All the color’s details should be available on a dedicated panel:
            - [x]	RGB Values are visible next to the sidebar and also in the panel
            - [x]	HEX, HSL, HSV, CMYK, XYZ, YIQ, YUV* are also visible in this panel
            - [x]	Hovering the values should show a little copy button alongside with a hover effect where the background color changes slightly
            - [x]	Clicking on each value should copy them to the clipboard of the user.
        - [x]	Add the possibility to bookmark the color/save it for later.
    - [x]	Chromatic wheel
        - [x]	Possibility to select a color from the chromatic wheel.
        - [x]	Possibility to select different chromatic wheels.
- [x] Color Tools
    - [x] Converter
        - [x] Possibility to enter a color using a specified color type thanks to a dropdown.
        - [x] Possibility to show the color converted in all color types (RGB, HEX, HSV, HSL, CMYK, YIQ, XYZ and YUV) inside of a color detail panel.
        - [x] A message should say to the user if the specified color is valid.
        - [x] Dedicated textboxes are shown for the user when the selected color type requires different values for each “variable”.
        - [x] Possibility to automatically paste a color in all the different textboxes.
        - [x] A preview of the color specified by the user should also be visible.
        - [x] Add the possibility to bookmark the color/save it for later.
    - [x] Text
        - [x] Possibility to set the font and the font size.
        - [x] Possibility to set the text foreground color and background color.
        - [x] Possibility to get the contrast ratio between the foreground and background color. -> The ratio will be indicated by an arrow alongside a scale.
- [x] Creation
    - [x] Palette
        - [x] Possibility to get different shades of a specified color.
        - [x] Possibility to get different brightness of a specified color.
        - [x] Possibility to get different hues of a specified color.
        - [x] Possibility to bookmark the color palette/save it for later.
        - [x] Possibility to set the color in every color type.
    - [x] Gradient
        - [x] Possibility to generate a random gradient with two stops.
        - [x] Possibility to change the rotation of the gradient.
        - [x] Possibility to export the gradient as CSS.
        - [x] Possibility to bookmark the gradient.
- [x] Home
    - [x] Dashboard with most relevant features for the user
    - [x] Relevant actions are also displayed in a section of the dashboard
    - [x] Least relevant features for the user are displayed in the “Discover” section
- [x] Bookmarks
    - [x] The user interface features the following categories:
        - [x] Colors (regular colors)
        - [x] Palette (a set of generated colors)
        - [x] Gradient (the gradient with all its parameters)
    - [x] Possibility to remove a specific color.
    - [x] Possibility to remove all bookmarks.
- Settings
    - [x] Possibility to change the theme.
    - [x] Possibility to change the language.
    - [x] Possibility to set the default color type.
    - [x] Possibility to set the default page on start.
    - [x] Possibility to customize the keyboard shortcuts.
    - [x] Possibility to set the default values of the text tool.
    - [x] Possibility to manage the data:
        - [x] Reset settings.
        - [x] Import/Export settings.
        - [x] Enable/disable Synethia .
    - [x] About section with update settings and credits (licenses)
- [x] Main window
    - [x] The background is a mesh gradient with subtle colors.
    - [x] The window is resizable and responsive.
    - [x] Basic window behavior (maximize, restore, minimize, close)
    - [x] Possibility to pin the window (always on top)
    - [x] A side bar is present on the left side:
        - [x] Main features are inside of colorful squares (Home, Bookmarks and settings)
        - [x] A second category for other features is under the first one
        - [x] Each feature is grouped by category; the user can expand a category when clicking on the category button.
        - [x] All of the icons are animated.

## User Interface and User eXperience (UI/UX)
To recap, the user interface looks similar to the one used in InternetTest Pro. The title of the app is shown to user with a welcome message underneath. Each mesh gradient used for the background should adapt to the light or dark theme. Synethia manages the user experience.

## External tools and libraries
ColorPicker Max intends to use the long-term supported .NET 6 (LTS). The app is written using C# and the Windows Presentation Framework (WPF). The following libraries/NuGet packages will be used:
- ColorHelper, to manage the conversion between each color types.
- PeyrSharp, to avoid having to implement some basic methods.
- Synethia, to design a unique experience for each user (dashboard).
