# axis-vrc-osc-bridge
This is a proof-of-concept AXIS Tracker to VRChat OSC Bridge.
[AXIS](https://axisxr.gg/) is a new full body tracker system created by [
Refract Technologies](https://refract.gg/), this project aims to create an alternative tracker integration to [VRChat](https://hello.vrchat.com/) using the [AXIS SDK provided by Refract Technologies](https://github.com/Refract-Technologies/axis-sdk-unity/) and [OSC API supported by VRChat](https://docs.vrchat.com/docs/osc-trackers), other than SteamVR driver.


## Demonstration

Here is a footage that I first time testing the integration within Unity Editor and VRChat.

https://user-images.githubusercontent.com/4715281/209565306-588c7894-ded9-435f-97d7-508176fb39d3.mp4

In the actual build it does not need a Unity Editor, instead it is a standalone application (but still, the AXIS Control Center is required).

## How to Use
- Just plug in, calibrate and setup your AXIS controller as written in the [official documentation](https://axisxr.gg/user-guide-manuals-documentation/).
- Open the application downloaded from [release section](https://github.com/JLChnToZ/axis-vrc-osc-bridge/releases/latest).
- Ensure VRChat is running with [OSC enabled](https://docs.vrchat.com/docs/osc-overview#enabling-it), it can be the same computer or different computer within the same network, or even using Quest.
- Enter the local IP address and port of which computer/Quest device is running VRChat.
  - If it is same computer, you may leave it `127.0.0.1` which is localhost.
  - For Quest client, you can [check your IP address in your Wi-Fi settings or SideQuest](https://smartglasseshub.com/find-quest-mac-ip-address/).
  - The default OSC port for VRChat is `9000`, don't change the port field unless you have changed your port when launching VRChat.
- Click connect, you should be able to toggle the full body tracking section in VRChat menu now, then you can setup the FBT in VRChat as written in [official documentation](https://docs.vrchat.com/docs/full-body-tracking#using-full-body-tracking-in-vrchat) just like the trackers has paired in SteamVR.

## License
[MIT](LICENSE)
