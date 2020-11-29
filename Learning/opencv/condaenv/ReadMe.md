# Conda Environment setup

## Goal

Build environment to run openCV sample code

## Prerequestit

Install Anaconda from [link](https://anaconda.org) and download

## Activate conda env

    * Move to current folder.
    * Create Environment
    ```console
    $ conda create -f environment.yml
    ``` 
    * Activate command
    ```console 
    $ conda activate opencv4od
    ```
    * Test 
    ```console
    $ ipython
    [In [3]: import cv2
    [In [4]: cv2.__version__
    ```
    * Expected output
    ```console
    Out[4]: '4.1.0'
    ```
