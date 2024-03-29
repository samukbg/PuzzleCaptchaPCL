## PuzzleCaptchaPCL

PuzzleCaptchaPCL is a simple cross-platform plugin for creating a draggable Puzzle Captcha for Xamarin and Xamarin Forms

### Setup
* Available on NuGet: http://www.nuget.org/packages/PuzzleCaptchaPCL [![NuGet](https://img.shields.io/nuget/v/PuzzleCaptchaPCL.svg?label=NuGet)](https://www.nuget.org/packages/PuzzleCaptchaPCL)
* Install into your .NET Standard project and Client projects.

### Screenshots
<div>
<img src="https://github.com/samukbg/PuzzleCaptchaPCL/blob/main/Screenshots/screenshot1.png?raw=true" alt="local/remote" width="200"/>
<img src="https://github.com/samukbg/PuzzleCaptchaPCL/blob/main/Screenshots/screenshot2.png?raw=true" alt="local-example" width="200"/>
<img src="https://github.com/samukbg/PuzzleCaptchaPCL/blob/main/Screenshots/screenshot3.png?raw=true" alt="remote-example" width="200"/>
</div>

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 7+|
|Xamarin.Android|API 14+|
|Xamarin.Mac|Untested|

### Implementation

You can implement the puzzle captcha by first importing the plugin via Xaml:
```xml
    xmlns:captcha="clr-namespace:PuzzleCaptchaPCL;assembly=PuzzleCaptchaPCL"
```

And then implement the captcha itself with the following options:
```xml
    <captcha:PuzzleCaptcha
        x:Name="captchaStack"
        ImageCollection="{Binding ImageCollection}"
        SliderThumbImage="sliderThumb.png"
        IsRemote="true"
        ReloadButtonImage="reloadIcon.png" />
```

### Properties
- ImageCollection: as a bindable ObservableCollection<object>
- SlideThumbImage: string type for changing slider button image
- ReloadButtonImage: for the captcha reload button
- IsRemote: boolean for setting the ImageCollection as url and not image sources

### License
Licensed under MIT, see license file.
```
//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
```

### Want To Support This Project?
All I have ever asked is to be active by submitting bugs, features, and sending pull requests!
