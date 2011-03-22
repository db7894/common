============================================================
Skeleton
============================================================

This is skeleton scala project.

============================================================
Building
============================================================

In order to build wdm, you will need the following::

    * some jdk (your pick, but tested on Oracle's...)
    * git (to pull down the source, or just download)
    * ant (to run the build files)

With those dependencies met, this will get you running from
start to finish::

    git clone https://github.com/bashwork/wdm.git
    cd wdm
    ant resolve
    ant package && ./config/runner.sh

============================================================
License
============================================================

Steal and be happy.
