# axis-vrc-osc-bridge
This is a proof-of-concept AXIS Tracker to VRChat OSC Bridge.
[AXIS](https://axisxr.gg/) is a new full body tracker system created by [
Refract Technologies](https://refract.gg/), this project aims to create an alternative tracker integration to [VRChat](https://hello.vrchat.com/) using the [AXIS SDK provided by Refract Technologies](https://github.com/Refract-Technologies/axis-sdk-unity/) and [OSC API supported by VRChat](https://docs.vrchat.com/docs/osc-trackers), other than SteamVR driver.


## Demonstration

Here is a footage that I first time testing the integration within Unity Editor and VRChat.

https://user-images.githubusercontent.com/4715281/209565306-588c7894-ded9-435f-97d7-508176fb39d3.mp4

In the actual build it does not need a Unity Editor, instead it is a standalone application (but still, the AXIS Control Center is requied).

## How to Use
- Just plug in, calibrate and setup your AXIS controller as written in the official documentation.
- Open the application downloaded from release section.
- Ensure VRChat is opened, it can be the same computer or different computer, or even using Quest.
- Enter the IP address of which computer/Quest device is running VRChat, if it is the same computer, you may leave it `127.0.0.1` which is localhost.
- Click connect, you should be able to toggle the full body tracking section in VRChat menu now.

## License
[MIT](LICENSE)
