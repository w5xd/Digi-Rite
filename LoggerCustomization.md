# Digi-Rite customization for a logger
Notes for customizing DigiRite to use a different logging program.

The implementation presented here uses WriteLog's published API
for logging, duping, and rig control. Changing to a different logger
involves two software considerations:

1) an implementation of the DigiRiteLogger.IDigiRiteLogger interface 
that calls the logger of your choice
2) code to instance your implementation object of IDigiRiteLogger.

For WriteLog, item (1) is accomplished in the DigiRiteLogger assembly.
You need to add an implementation of your own for IDigiRiteLogger.
Item (2) is accomplished via COM-specific Ft8Auto class and the Program class
that registers Ft8Auto for COM. 

The source files of interest for logger customization are:
<ol>
<li>DigiRiteLogger project.<br/>
You need to add your own class that implements IDigiRiteLogger.</li>
<li>Program.cs<br/>
For WriteLog, DigiRite is built into its own exe and with class Program
serving as the startup main. You may instead discard Program.cs and
link the remaining code into your own logger. (NOTE: GPL will then
require you to publish your source code!)</li>
<li>Ft8Auto.cs<br/>
WriteLog invokes DigiRite via COM. Its Program.cs and Ft8Auto.cs
together that make that happen.</li>
<li>MainForm.cs<br/>
WriteLog invokes the SetWlEntry method on Ft8Auto, which forwards
to the eponymous method on MainForm. </li>
</ol>

For reference in case you want to use a similar startup sequence, the 
WriteLog-specific startup for item (2) is:
<ol type="a">
    <li>DigiRite.exe is registered as a COM automation server.
         See Register.exe in this solution.</li>
    <li>DigiRite's COM GUID for Ft8Auto is installed  to the place in
         the registry that WriteLog looks for instancing an object from
         its right mouse click in its Entry WIndow. see Register.exe</li>
    <li> On WriteLog's Entry Window right mouse, it looks in that
         WriteLog-specific place in the registry for COM GUID,
         and calls CoCreateInstance on that GUID, looking for IDispatch.</li>
     <li> DigiRite's COM registration (see item a) causes Windows to
         start DigiRite.exe -EMBEDDING.</li>
    <li> COM instances an Ft8Auto object per the code in Program.cs</li>
    <li>WriteLog calls SetWlEntry on the create object</li>
</ol>

and away we go.
     
Also see the #region  Logger customization in MainForm.cs

This author, W5XD, is indifferent to your choice of whether to retain
binary compatiblity so that your customization can serve both WriteLog
and your customization, or whether you remove the WriteLog-specific 
classes and replace them with your own. Of course, if you remove the 
WriteLog-specific code here, kindly refrain from having your users
running an unmodified Register.exe to point WriteLog's registry entries
at your DigiRite.exe

