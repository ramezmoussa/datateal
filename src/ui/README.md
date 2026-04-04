### Compiling the UI CSS

The UI uses the Bootstrap frontend toolkit (CSS, JavaScript and icons) and the Blazor.Bootstrap NuGet package for reusable Blazor components. The app CSS is generated from the Bootstrap SCSS files using SASS. This allows for easier customization of the Bootstrap theme. The following guide describes how to compile the CSS using SASS.

#### Install SASS

1. Install [Node.js](https://nodejs.org/en/download)
2. Install [SASS](https://sass-lang.com/install) using npm
   - `npm install -g sass`

#### Compile the CSS

1. In a terminal, navigate to the `DuckHouse.Ui/wwwroot/css` folder
2. Run `sass bootstrap.custom.scss:bootstrap.custom.css`