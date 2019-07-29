# SetIcon
Command line tool that can set the icon of another window

## examples


### Set notepad window icon
``` Powershell
.\SetIcon.exe -image C:\path\to\image.png -title "Untitled - Notepad"
```

### Set window icon by window handle
``` Powershell
.\SetIcon.exe -image C:\path\to\image.png -handle 0x112269
```