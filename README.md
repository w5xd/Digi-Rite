# Digi-Rite
Digital mode implementation based on FT8.

Copyright (c) 2019 by WriteLog Contesting Software, LLC

DigiRite is a radio contesting accessory for FT8 messaging. It optionally integrates with WriteLog, but also can run stand alone. It is published open source. Other contest logging program authors are free to use DigiRite's sources to build their own integrated FT8 solution under the terms of the GPL.

<blockquote>
The algorithms, source code, look-and-feel of WSJT-X and related programs, and protocol specifications for the modes FSK441, FT8, JT4, JT6M JT9, JT65, JTMS, QRA64, ISCAT, MSK144 are Copyright (C) 2001-2018 by one or more of the following authors: Joseph Taylor, K1JT; Bill Somerville, G4WJS; Steven Franke, K9AN; Nico Palermo, IV3NWV; Grea Bream, KI7MT; Michael Black, W9MDB; Edson Pereira, PY2SDR; Philip Karn, KA9Q; and other members of the WSJT Development Group.
</blockquote> 

If you want to build DigiRite from these sources the
prerequisites are:
<ul>
<li>Visual Studio 2017. The Community Edition (free) or any other.
<li>Use VS to build the solution in submodule Digi-XDft/XDft.sln. Build all the configurations and platforms thatyou also want for this DigiRite build. That XDft solution requires the next item on this list:
<li>A build of the git repo at https://github.com/w5xd/Digi-XDwsjt. 
That repo needs various third party components to build, or, alternatively,
you can fetch the XDwsjtFT8SDk-<version-number>.7z already built.
</ul>
To get all the sources, you must, in addition to git pull of this repo, also pull
its submodules:
<p><code>git submodule update --init --recursive</code></p>

After the above are complete, Visual Studio should build the DigiRite.sln here.

