# ImageOrganizer

You all know the problem that your photos are not sorted like they should be. They are all in different folders, have inaccurate and not matching naming are not accessible when you need them. To minimize search time and keep you calm use **Image Organizer** to organize your images.

Check out the [Releases](https://github.com/aschbacd/ImageOrganizer/releases) to download the final builds.

## What it does

The program has to be provided with three directories - source, destination and error.

With a click on **Scan** your source folder and all subdirectories it contains will be scanned for files that will be shown afterwards in the listbox. By checking **Automatically start after scanning** the program immediately starts organizing after the scanning process. This action can also be achieved by clicking the **Start** button manually.

Image Organizer recursively scans the earlier mentioned source folder and checks every single file for image meta data. If the file type is supported (image files) and it contains all information needed (images dates) the image will be placed in an appropriate folder structure.

### Example

**Input file:** test-image_170701.jpg

**Output file:** 2017 / 07 July / IMG_20170701_120530.JPG

![Screenshot 01](Screenshots/Screenshot_01.png)
