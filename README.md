# Digi-Rite
Digital mode implementation for FT8.

Copyright (c) 2020 by WriteLog Contesting Software, LLC

DigiRite is a radio contesting accessory for FT8 messaging. It optionally integrates with WriteLog, but also can run stand alone. It is published open source. Other contest logging program authors are free to use DigiRite's sources to build their own integrated FT8 solution under the terms of the <a href='COPYING'>GPL</a>.

<blockquote>
The algorithms, source code, look-and-feel of WSJT-X and related programs, and protocol specifications for the modes FSK441, FT8, JT4, JT6M JT9, JT65, JTMS, QRA64, ISCAT, MSK144 are Copyright (C) 2001-2018 by one or more of the following authors: Joseph Taylor, K1JT; Bill Somerville, G4WJS; Steven Franke, K9AN; Nico Palermo, IV3NWV; Grea Bream, KI7MT; Michael Black, W9MDB; Edson Pereira, PY2SDR; Philip Karn, KA9Q; and other members of the WSJT Development Group.
</blockquote> 

To find an already-built installer for DigiRite, checkout email reflector at https://groups.io/g/digirite. Several hams build it regularly and some make the msi file available for public download.

DigiRite is designed as a plug-in component supporting FT4 and FT8 and published such that any ham radio logbook program is permitted to use it subject to the software license held by the WSJT-X copyright holders (see https://sourceforge.net/projects/wsjt/). WriteLog is such a logbook program. There is also the Trivial ADIF logbook at http://github.com/w5xd/TrivialAdifLogger. Trivial supports DigiRite, has rig control (courtesy http://github.com/hamlib) and uses ADIF as its native file format. Use your favorite ADIF-capable logbook program with Trivial's ADIF.

If you want to build DigiRite from these sources, its prerequisites are:
<ul>
<li>Visual Studio 2017. The Community Edition was used by this author.</li>
<li>Use VS to build the solution in submodule Digi-XDft/XDft.sln. Build all the configurations and platforms that you also want for this DigiRite build. That XDft solution, in turn, requires the next item on this list:</li>
<li>A build of the git repo at https://github.com/w5xd/Digi-XDwsjt. 
  That repo needs various third party components to build (see its <a href='https://github.com/w5xd/Digi-XDwsjt'>git repo</a>), or, alternatively,
  you can fetch the XDwsjtFT8SDk-&lt;version-number&gt;.7z already built. (Again, see that <a href='https://github.com/w5xd/Digi-XDwsjt'>same repo</a>.)</li>
  <li>The InstallDigiRite project builds an msi installer, and is optional. For it to build you will need the WIX installer at https://wixtoolset.org/releases</li>
</ul>
To get all the sources, you must, in addition to git pull of this repo, also pull
its submodules:
<p><code>git submodule update --init --recursive</code></p>

After the above are complete, Visual Studio should build the DigiRite.sln in this repository.

<h3>Software Architecture Overview</h3>
There are three separate layers, each published in its own repository.
<ol>
<li>The repo at <a href='https://github.com/w5xd/Digi-XDwsjt'>Digi-XDwsjt</a> is the only dependence from DigiRite on the WSJT-X sources. Outside of its dependence on the subset of WSJT-X sources and certain build tools, Digi-XDwsjt stands alone, architecture-wise. Digi-XDwsjt builds only the encoding and decoding FT8 algorithms. Specifically, none of the rig control, log-keeping, user interface, etc. of WSJT-X is incorporated in that project nor used in DigiRite. </li>
<li>The repo at <a href='https://github.com/w5xd/Digi-XDft'>Digi-XDft</a> depends on Digi-XDwsjt and wraps the WSJT-X FT8 encode/decode in .NET callable wrappers. Digi-XDft also contains some (MIT-licensed) utilities for send/receiving sound on Window Sound Devices. This layer depends on the layer above, and also the Microsoft Visual Studio .NET build tools.</li>
<li>Finally, this repo depends on Digi-XDft and is the source for a Windows application that sends/receives FT8 messages. This layer depends on the above.</li>
</ol>

