# UnturnedMultiplayerMapEditor

Multiplayer map editor for Unturned.

This project was discontinued because it's not worth the time spent for a few people who may use it. I decided to make
the code public for anyone interested in making something similar. I don't recommend using it on real maps in its
current state.

[Objects sync video](https://user-images.githubusercontent.com/37713088/230732770-a84ab441-cd46-4a55-98d1-c461a2993400.webm)

## Features

> :warning: Note that at this point nothing is synchronized when someone joins the server meaning that the host and all
> players should have the same map files and no changes should be made before all clients are connected.

Things that are currently synchronized over the network:

### Objects

- Add/Remove
- Move/Rotate/Scale
- Edit material override

### Terrain

- [WIP] Update height using any brush

## Usage

Build `MultiplayerMapEditor.Module` project. Copy output to `Unturned/Modules/MultiplayerMapEditor` folder.

To host a server select a map in the Main Menu -> Workshop -> Editor window and click the Host button.

To connect to a server click the Connect button in the Main Menu -> Workshop -> Editor window then enter IP (127.0.0.1)
and port (27015) and click Connect again.
