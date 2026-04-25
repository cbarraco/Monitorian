# Brightness

Brightness is a command line tool to control monitor brightness.

## Requirements

* Windows 7 or newer
* An external monitor must be DDC/CI enabled.

## License

* MIT License

## Usage

```text
brightness - Control monitor brightness from the command line

Usage:
  brightness list                   List all monitors with current brightness
  brightness get [<index>]          Get brightness of all or a specific monitor
  brightness set all <0-100>        Set brightness on all monitors
  brightness set <index> <0-100>    Set brightness on a specific monitor

Examples:
  brightness list
  brightness set all 50
  brightness set 0 75
  brightness get 1
```

## Acknowledgements

This project is based on [Monitorian](https://github.com/emoacht/Monitorian) by [emoacht](https://github.com/emoacht), a Windows desktop tool for adjusting monitor brightness. The monitor management and DDC/CI integration code is derived from that project.
