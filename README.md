# ImageOrganizer

You all know the problem that your photos are not sorted like they should be. They are all in different folders, have inaccurate / not matching naming and are not accessible when you need them. To keep yourself organized and minimize search time in many situations use **ImageOrganizer**.

Check out the [Releases](https://github.com/aschbacd/ImageOrganizer/releases) to download the final builds.

## What it does

The program has to be provided with three directories:

* source directory
* destination directory
* error directory

By clicking **Scan** your source folder (including all subdirectories) will be scanned for files that are being showed afterwards in the list box below to check for correctness.

By checking **Automatically start after scanning** the program immediately starts organizing after the scanning process. This action can also be achieved by clicking the **Start** button manually.

Image Organizer recursively scans the earlier mentioned source folder and checks every single file for image meta data. If the file type is supported `(image files)` and it contains all information needed `(images dates)` the image will be checked for duplication errors meaning an "exact" copy has been organized before.

`Exact being in quotation marks because of the options to disclaim checksums.`

## Detect duplicates using checksums

When using the checksum option, image duplicates can be identified not only by their time taken but also by their contents, meaning their checksums. Currently these options are available:

* NONE
* MD5
* SHA1
* SHA256
* SHA512

### Thanks to

* Freepik (provided icon)
* GitHub community
