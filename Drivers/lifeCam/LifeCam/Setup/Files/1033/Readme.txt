Microsoft LifeCam Readme File
May 2007
(c) 2007 Microsoft Corporation. All rights reserved.

This document provides late-breaking or other information that supplements the Microsoft LifeCam software documentation.

For detailed information about LifeCam software, see Microsoft LifeCam Help, installed with the LifeCam software.

>>>To open Microsoft LifeCam Help, click Start, click All Programs, click Microsoft LifeCam, and then click LifeCam Help.

--------------------------------------
How to Use This Document
--------------------------------------

To view the Readme file on-screen in Notepad, maximize the Notepad window. On the Format menu, click Word Wrap. To print the Readme file, open it in Notepad or another word processor, and then use the Print command on the File menu.

-------------
CONTENTS
-------------

1.0  Installation and Updates
2.0  Hardware
3.0  Video capture
4.0  Instant Messaging programs
5.0  Other applications
6.0  Error messages
---------------------



1.0  Installation and Updates
---------------------

1.1  “To install the Microsoft LifeCam software, you must first install an update”

You may receive the following message during Setup:
"To install the Microsoft LifeCam software, you must first install an update. 
You can download the update from the Microsoft Web site (KB822603)."

NOTE: If you do NOT receive the message, Setup has detected that you do NOT need this update, or you already have it.

>>To fix this:
1. Go to http://support.microsoft.com/support.
2. Search for KB822603.
3. Click the "Download the 822603 package now" link.
4. Follow the instructions to download and install the update.

After you install the update, restart LifeCam Setup to install the LifeCam software. 

---------------------

1.2  "Installation error"

On Windows Vista N and KN versions, you may receive the following message during Setup:
"Installation error"

>>To fix this:
1. Go to http://support.microsoft.com/support.
2. Search for KB929399.
3. Click the link to download the Windows Media Format 11 SDK for your computer.
4. Follow the instructions to download and install the update.

After this installation is complete:
   Go to the C:\WMSDK\WMFSDK11\Redist directory, and install the SDK program in this directory.


---------------------

1.3  Installation error message: Not enough disk space or an error has occurred.

You may receive this error message for multiple reasons:
- Installation can be blocked by a virus-protection, a pop-up-blocker, or a spyware-detection program. (For more information, see 1.4.)
- The hard drive where you are installing the software may be full.  

---------------------

1.4  Installation can be blocked by virus-protection, pop-up-blocker, or spyware-detection programs

During installation of the LifeCam software, your virus protection program, pop-up blocker, or spyware detection program may block the installation process. 

Many of these programs will ask you whether to continue blocking or to allow installation. 
If prompted, allow the software to install. 

>>To fix this:
- Try temporarily exiting your virus protection program and running LifeCam Setup again.

---------------------
 
1.5  To complete the Automatic Update installation, close and then open LifeCam software again
 
After you have downloaded and installed an Automatic update, you must close the LifeCam software and then open it again for the changes to take effect.

For some updates, you may also be prompted to reboot your computer before changes will take effect.

---------------------

1.6  After installing Logitech® webcam software, Microsoft LifeCam software must be reinstalled (Code 39 error message)

The Logitech webcam software installation process can cause problems with the Microsoft LifeCam software, including a Code 39 error message.

To install both the Microsoft LifeCam software and Logitech QuickCam software on the same computer, install the Logitech QuickCam software before installing the Microsoft LifeCam software.  

When using the Microsoft LifeCam, you may also need to shut down the Logitech QuickCam software or disable it from the system tray.

---------------------

1.7  After uninstalling LifeCam software, drivers are removed.

In Windows Vista, after uninstalling the Microsoft LifeCam software, the drivers for the Microsoft LifeCam are removed.

>>To return the LifeCam drivers after uninstalling the software:
- Plug in the webcam. The Add New Hardware Wizard will appear.
- Click Browse and navigate to the installation directory (example: C:\Program Files\Microsoft LifeCam) to locate and add the drivers.




---------------------
2.0  Hardware
---------------------


2.1  About Universal Serial Bus (USB) ports

If your webcam does not function correctly with the USB port to which it is connected, try plugging the webcam into a different USB port.

Your Microsoft LifeCam will work on either a USB 1.1 or USB 2.0 port. 

USB 1.1 and USB 2.0 ports look identical.  If possible, use a USB 2.0 port to connect your LifeCam webcam. Some webcam features may not be supported on USB 1.1 ports.

Different USB ports may also provide different amounts of power to your webcam. A USB hub on a keyboard, for example, may not have sufficient power for the LifeCam webcam. You may need to plug the webcam into a USB port on the computer itself, or into a powered USB hub.


---------------------

2.2  Webcam may perform poorly or not at all if too many USB devices are connected to the computer

If you have too many USB devices connected to the computer, the LifeCam webcam may perform poorly or stop operating. 

Symptoms might include: the video area is black (no picture or sound), Windows Live Messenger stops working (hangs) when you start a video call, or the webcam won't work in high-resolution videos.

>>To fix this:
- Try disconnecting and reconnecting the LifeCam webcam.
- Try disconnecting other unused USB devices.
- Try using a different USB port on the computer.
- If using a USB hub, try unplugging the hub and plugging the webcam directly into your computer.

---------------------

2.3  LifeCam VX-6000 may not work with ATI Radeon 9600 card

When using the ATI Radeon 9600 video card, the video capture from the LifeCam VX-6000 may freeze or flicker. 

---------------------

2.4  Video may display as a full-screen image

Some video cards include features to display video as a full-screen image.

If you prefer not to see LifeCam video in full-screen mode, this feature can usually be disabled.  See the hardware manufacturer documentation for instructions about disabling or turning off this feature.

---------------------

2.5  LifeCam may crash when starting and stopping video on a second monitor

If you use multiple monitors, and run LifeCam from a secondary monitor, the LifeCam software may crash.

>>To prevent this error:
 - Run the LifeCam software in the primary monitor.





---------------------
3.0  Video capture
---------------------

3.1  Not all resolutions supported when using USB 1.1 port

When connected to a USB 1.1 port, LifeCam products support a maximum video resolution of 800 x 600 pixels, and a maximum photograph resolution of 1.9 megapixels. 

>>To fix this:
- If available, connect to a USB 2.0 port.
  - Or -
- Lower the resolution.

---------------------

3.2  Video resolution does not support pan, tilt, zoom, and face tracking when using maximum resolution

When the highest resolution is set for the webcam, pan, tilt, zoom, and face tracking features are not available. To use pan, tilt, zoom, or face tracking, choose a lower video resolution.

---------------------

3.3  LifeCam NX-6000 -- preview capture may appear black

In some cases, the preview capture may not appear.

>>To fix this:
- Unplug the webcam and plug it back in.
  - Or -
- Lower the video resolution.

---------------------

3.4  The capture screen stays in the foreground when using generic video drivers

In some cases, the video capture will remain in the foreground, even if you open other windows on top of the LifeCam software.
This may happen if using generic drivers for your video display.

>>To fix this:
- See the hardware manufacturer documentation for information about how to update your video drivers.




---------------------
4.0  Instant Messaging programs
---------------------

4.1  Microsoft LifeCam software may be required for certain messaging clients

To ensure product compatibility with some messaging clients, such as Skype and Yahoo! Messenger, the Microsoft LifeCam software may be required.

---------------------

4.2  Video effects do not work in all Instant Messenger programs

Some video effects will not display in instant messenger programs, such as Skype.

---------------------

4.3  Video calls in America Online (AOL) Instant Messenger may disconnect

If you are behind a firewall, video calls established with AOL Instant Messenger may disconnect shortly after starting.

>> For more information about adjusting for firewall or router settings, see www.aim.com.


---------------------

4.4  No microphone available in video calls in Windows Live Messenger.

On Windows Vista N and KN versions, using Windows Live Messenger, the microphone volume may be disabled and unadjustable.

>>To fix this:
1. Go to http://support.microsoft.com/support.
2. Search for KB929399.
3. Click the link to download the Windows Media Format 11 SDK for your computer.
4. Follow the instructions to download and install the update.

After this installation is complete:
   Go to the C:\WMSDK\WMFSDK11\Redist directory, and install the SDK program in this directory.




-----------------------
5.0  Other applications
-----------------------

5.1  LifeCam VX-6000 may not appear in the Microsoft Office Communicator Audio and Video Tuning Wizard. 

In Microsoft Office Communicator, the LifeCam is set up through the Audio and Video Tuning Wizard.  In some cases, the LifeCam VX-6000 may not appear in the list of choices.

>>To fix this:
-  Plug in the LifeCam webcam before turning the computer on.

---------------------

5.2  When posting to Windows Live Spaces, I see an "Already signed in" page or HTML code instead of my personal space

If you post photos to Windows Live Spaces accounts using two user accounts on the same computer, Windows Live Spaces will provide an "Already signed in" error.

>>To fix this:
1.  Close all Internet Explorer browser windows. 
2.  In the LifeCam software, click the "Post to my blog in Windows Live Spaces" button again. 

>>To prevent this error:

- Close Internet Explorer between user sessions.

---------------------

5.3  If one user changes the default webcam, the default webcam is changed for all users.

The LifeCam software settings may affect all user profiles.  For instance, if you change the default webcam in one user profile, the default webcam will change for all user profiles on that computer.




---------------------
6.0  Error messages
---------------------

6.1  "Not enough hard disk space" error when LifeCam Files folder is missing

If the "My Documents\LifeCam Files" folder is deleted, or the location of the folder becomes unavailable (for example, over a network share), your video clips will not be saved and you may encounter the error message "There is not enough free hard disk space to capture a photo, audio clip, or video clip. Increase the amount of free space on hard disk and then try again." 

>>To fix this:
- Recreate the "My Documents\LifeCam Files" folder.
  - Or -
- Close the LifeCam software and then reopen it.  The folder will be automatically recreated.

---------------------

6.2  System crashes, with serious error in Tiptsf.dll

Your computer may receive a serious error (STOP in Tiptsf.dll). 

>>>To fix this:
Download Windows XP Service Pack 2 (SP2).  
1.  Go to http://update.microsoft.com.
2.  Download and install Windows XP Service Pack 2 (SP2).


---------------------

6.3  LifeCam VX-6000: Blue screen in Usbport.sys

Your computer may receive a serious error (STOP in Usbport.sys). 

>>>To fix this:
Download Windows XP Service Pack 2 (SP2).  
1.  Go to http://update.microsoft.com.
2.  Download and install Windows XP Service Pack 2 (SP2).


---------------------

6.4  LifeCam software crashes when opening the software (Nero Video Decoder)

This can occur when certain versions of Nero Video Decoder are installed.

Updating or uninstalling the software will fix the problem.  

See the hardware manufacturer documentation for information about how to update your software.


---------------------

6.5  LifeCam software crashes when opening the software. (Pegasus PICVideo)

This can occur when certain versions of Pegasus PICVideo are installed.

Uninstalling Pegasus PICVideo will solve the problem.

You can also check with the manufacturer to see if updates are available.


======================