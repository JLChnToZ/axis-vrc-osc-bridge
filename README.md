# AXIS VRC OSC Bridge
This is a proof-of-concept AXIS Tracker to VRChat OSC Bridge.
[AXIS](https://axisxr.gg/) is a new full body tracker system created by [
Refract Technologies](https://refract.gg/), this project aims to create an alternative tracker integration to [VRChat](https://hello.vrchat.com/) using the [AXIS SDK provided by Refract Technologies](https://github.com/Refract-Technologies/axis-sdk-unity/) and [OSC API supported by VRChat](https://docs.vrchat.com/docs/osc-trackers), other than SteamVR driver.

The difference between official SteamVR driver and this bridge:
- The bridge runs standalone and itself don't require SteamVR.
- The bridge only supports VRChat (or any application speaks the same OSC protocol as VRChat).
- The bridge can connect to different computer or even Quest clients.
- The bridge don't require you to enter limb lengths in AXIS Control Center.

## Demonstration
https://user-images.githubusercontent.com/4715281/215272407-1b285307-bc83-4452-93d5-09cd29c43f0d.mp4

## How to Use
- Connect and set up your AXIS controller as instructed in the [official documentation](https://axisxr.gg/user-guide-manuals-documentation/).
- Open the application downloaded from the [release section](https://github.com/JLChnToZ/axis-vrc-osc-bridge/releases/latest).
  - Beware SteamVR drivers seems conflict with the bridge, if either one is running, the other will not work. If you launches SteamVR with the drivers installed **before** the bridge, the bridge may not be able to receive tracker data. In this case you may need to disable/uninstall the driver first.
- Make sure VRChat client is running with [OSC enabled](https://docs.vrchat.com/docs/osc-overview#enabling-it). The client can be running on the same computer or a different computer within the same network, or even using Quest.
- Enter the local IP address and port of the computer/Quest device running VRChat.
  - If it's the same computer, you can leave it as `127.0.0.1` (localhost).
  - For Quest client, you can [check your IP address in your Wi-Fi settings or SideQuest](https://smartglasseshub.com/find-quest-mac-ip-address/).
  - The default OSC port for VRChat is `9000`. Don't change the port field unless you changed it when launching VRChat.
- Click connect. You should be able to toggle the full body tracking section in the VRChat menu now. Then, you can set up FBT in VRChat as instructed in the [official documentation](https://docs.vrchat.com/docs/full-body-tracking#using-full-body-tracking-in-vrchat), just like the trackers have paired in SteamVR.
- If your AXIS controller is set up with 16 nodes, you can try enabling "Track Head Position" for more accurate head tracking. Don't turn it on if you don't have 16 nodes set up.
- If your body and head orientation is not synced and you don't have 16 nodes set up, you can click the "Sync Body & VR Headset Facing" button and then look straight ahead for 3 seconds to calibrate.

## License
[MIT](LICENSE)
