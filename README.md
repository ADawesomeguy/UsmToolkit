# UsmToolkit

Tool to convert USM video files into user-friendly formats.

## Getting started

To begin with, make sure to install the depedencies `ffmpeg` and `vgmstream`. This is platform-specific and can be figured out easily enough.

After that, it's as easy as it can get.

### Extracting
```
UsmToolkit extract <file/folder>
```

### Converting
```
UsmToolkit convert <file/folder>
```

For more informations run `UsmToolkit extract -h` and `UsmToolkit convert -h`.

## Custom conversion parameter

You should find `config.json` in the folder of the executable. With it, you can completly customize how the extracted file is processed by ffmpeg.
The default configuration ships as follows:

* Video: Will be copied
* Audio: Re-encoded as AC3 at 640kb/s. If the file has 6 channels, they will be merged into stereo
    * Left channel: CH1, CH3, CH5 50% volume, CH6
    * Right channel: CH2, CH4, CH5 50% volume, CH6
* Output is a MP4 file

You can change these settings to your likings, it's standard ffmpeg syntax.

## License

UsmToolkit follows the MIT License. It uses code from [VGMToolbox](https://sourceforge.net/projects/vgmtoolbox/).
