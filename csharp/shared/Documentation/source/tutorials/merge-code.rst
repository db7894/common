============================
Merging Source Code
============================
 
.. note :: Before attempting to merge, check in (or undo) all pending changes.
   This ensures that no data will be lost during the merge. The target branch must
   be mapped to a local folder on the machine running Visual Studio.
 
------------------------
Merging
------------------------

Navigate to View->Other Windows->Source Control Explorer

.. image:: ../includes/images/merge-1.jpg

In the folders panel of the Source Control Explorer Window, navigate to cgtmfndation001->MarketData->Trunk 
Right-click the Trunk folder and choose Merge...

.. image:: ../includes/images/merge-2.jpg

In the resulting Merge Wizard, ensure that the Source Branch is Trunk, select the "All changes up
to a specific version" option, and select the desired target branch from the dropdown menu. Then click Next.

.. image:: ../includes/images/merge-3.jpg

Select Latest Version for the Version Type and click Finish.

.. image:: ../includes/images/merge-4.jpg

If there are no conflicts Visual Studio will conduct the merge automatically. If conflicts are detected,
a Resolve Conflicts dialog will appear.  Resolving Conflicts

Select a conflict from the list and click Resolve...

.. image:: ../includes/images/merge-5.jpg

For source files, conflicts can be viewed side-by-side by clicking the Compare... button at the bottom left.
For most other files, choose "Copy item from source branch" and click OK.

.. image:: ../includes/images/merge-6.jpg

Once each conflict is resolved, click Close to close the Resolve Conflicts box and complete the merge.
