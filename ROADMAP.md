# Roadmap for ColorPicker Max
![Banner](https://github.com/Leo-Corporation/LeoCorp-Docs/raw/master/Images/ColorPicker%20Max%20Banner.png)

## Introduction
ColorPicker Max, codename “Project Spectrum” is the next major iteration of Léo Corporation’s popular software ColorPicker. This new version will be written using the latest features of .NET and C#. It will be a complete rewrite of ColorPicker from the ground up. It will include a brand-new designed based on InternetTest Pro’s. It will also feature an integration with the newly launched Synethia algorithm library.

## Features
- [ ]	Picker
    - [ ]	Selector
        - [ ]	Possibility to select a specific color using different sliders.
        - [ ]	Possibility to select a color from a pixel on the screen.
        - [ ]	Possibility to choose a color randomly.
        - [ ]	All the color’s details should be available on a dedicated panel:
            - [ ]	RGB Values are visible next to the sidebar and also in the panel
            - [ ]	HEX, HSL, HSV, CMYK, XYZ, YIQ, YUV* are also visible in this panel
            - [ ]	Hovering the values should show a little copy button alongside with a hover effect where the background color changes slightly
            - [ ]	Clicking on each value should copy them to the clipboard of the user.
        - [ ]	Add the possibility to bookmark the color/save it for later.
    - [ ]	Chromatic wheel
        - [ ]	Possibility to select a color from the chromatic wheel.
        - [ ]	Possibility to select different chromatic wheels.
- [ ] Color Tools
    - [ ] Converter
        - [ ] Possibility to enter a color using a specified color type thanks to a dropdown.
        - [ ] Possibility to show the color converted in all color types (RGB, HEX, HSV, HSL, CMYK, YIQ, XYZ and YUV) inside of a color detail panel.
        - [ ] A message should say to the user if the specified color is valid.
        - [ ] Dedicated textboxes are shown for the user when the selected color type requires different values for each “variable”.
        - [ ] Possibility to automatically paste a color in all the different textboxes.
        - [ ] A preview of the color specified by the user should also be visible.
        - [ ] Add the possibility to bookmark the color/save it for later.
    - [ ] Text
        - [ ] Possibility to set the font and the font size.
        - [ ] Possibility to set the text foreground color and background color.
        - [ ] Possibility to get the contrast ratio between the foreground and background color. -> The ratio will be indicated by an arrow alongside a scale.
- [ ] Creation
    - [ ] Palette
        - [ ] Possibility to get different shades of a specified color.
        - [ ] Possibility to get different brightness of a specified color.
        - [ ] Possibility to get different hues of a specified color.
        - [ ] Possibility to bookmark the color palette/save it for later.
        - [ ] Possibility to set the color in every color type.
    - [ ] Gradient
        - [ ] Possibility to generate a random gradient with two stops.
        - [ ] Possibility to change the rotation of the gradient.
        - [ ] Possibility to export the gradient as CSS.
        - [ ] Possibility to bookmark the gradient.
- [ ] Home
    - [ ] Dashboard with most relevant features for the user
    - [ ] Relevant actions are also displayed in a section of the dashboard
    - [ ] Least relevant features for the user are displayed in the “Discover” section
- [ ] Bookmarks
    - [ ] The user interface features the following categories:
        - [ ] Colors (regular colors)
        - [ ] Palette (a set of generated colors)
        - [ ] Gradient (the gradient with all its parameters)
    - [ ] Possibility to remove a specific color.
    - [ ] Possibility to remove all bookmarks.
- Settings
    - [ ] Possibility to change the theme.
    - [ ] Possibility to change the language.
    - [ ] Possibility to set the default color type.
    - [ ] Possibility to set the default page on start.
    - [ ] Possibility to customize the keyboard shortcuts.
    - [ ] Possibility to set the default values of the text tool.
    - [ ] Possibility to manage the data:
        - [ ] Reset settings.
        - [ ] Import/Export settings.
        - [ ] Enable/disable Synethia .
    - [ ] About section with update settings and credits (licenses)
- [ ] Main window
    - [ ] The background is a mesh gradient with subtle colors.
    - [ ] The window is resizable and responsive.
    - [ ] Basic window behavior (maximize, restore, minimize, close)
    - [ ] Possibility to pin the window (always on top)
    - [ ] A side bar is present on the left side:
        - [ ] Main features are inside of colorful squares (Home, Bookmarks and settings)
        - [ ] A second category for other features is under the first one
        - [ ] Each feature is grouped by category; the user can expand a category when clicking on the category button.
        - [ ] All of the icons are animated.

## User Interface and User eXperience (UI/UX)
To recap, the user interface looks similar to the one used in InternetTest Pro. The title of the app is shown to user with a welcome message underneath. Each mesh gradient used for the background should adapt to the light or dark theme. Synethia manages the user experience.

## External tools and libraries
ColorPicker Max intends to use the long-term supported .NET 6 (LTS). The app is written using C# and the Windows Presentation Framework (WPF). The following libraries/NuGet packages will be used:
- ColorHelper, to manage the conversion between each color types.
- PeyrSharp, to avoid having to implement some basic methods.
- Synethia, to design a unique experience for each user (dashboard).