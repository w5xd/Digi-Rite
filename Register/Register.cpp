// Make appropriate Windows registry entries for DigiRite
//
// This is NOT the preferred method of installing DigiRite!
//  A standard Microsoft installer, .msi, that can be uninstalled is preferred.
// Kit for such an installer is included here in InstallDigiRite.
//  BUT...that kit requires more than just Visual Studio installed...
// so this program is provided as an alternative...
//
// Recommendation to builder of DigiRite:
//  Decide which deployment method to use: 
//  Build the msi with the InstallDigiRite project and distribute the resulting msi file.
//  OR
//  Bundle the exe's into a zip file along with this Register.exe and distribute
//  the resulting zip file.
//
// This Register.exe and the DigiRiteOpenSource.msi file can both be invoked
// on the same PC but they work in "last one wins" mode. WriteLog will invoke
// whichever one was most recently installed.
//
// Precondition: 
//      Register.exe is in the same directory and 
//      is the same architecture (x86 vs x64) as DigiRite.exe
//  The x64 build will register, but it won't work until and unless WriteLog
//  installed an x64 build of its waterfall assembly.
//

#include "pch.h"

#if !(defined(_DEBUG))
static const HKEY ClassesRoot = HKEY_CLASSES_ROOT;
static const HKEY LocalMachine = HKEY_LOCAL_MACHINE;
#else   // for debugging, don't use protected part of registry. just test
static HKEY ClassesRoot;
static HKEY LocalMachine;
#endif

int main(int argc, TCHAR argv[])
{
    static const unsigned FNSIZE = 512;
    std::vector<wchar_t> fn(FNSIZE);
    auto fnl = ::GetModuleFileNameW(0, &fn[0], FNSIZE);
    std::wstring fns;
    fns.assign(fn.begin(), fn.begin() + fnl);
    auto finalbSlash = fns.rfind('\\');
    if (finalbSlash != fns.npos)
    {
        static const wchar_t ClsidStr[] = L"CLSID\\{4CC973B6-9BFB-3E4B-8157-E55774DD50C0}";
        static const wchar_t clsid[] = L"{4CC973B6-9BFB-3E4B-8157-E55774DD50C0}";

#if defined(_DEBUG)
        ::RegCreateKeyExW(HKEY_CURRENT_USER, L"TestRegisterExe\\HKCR", 0, 0, 0, KEY_READ|KEY_WRITE, 0, &ClassesRoot, 0);
        ::RegCreateKeyExW(HKEY_CURRENT_USER, L"TestRegisterExe\\HKLM", 0, 0, 0, KEY_READ|KEY_WRITE, 0, &LocalMachine, 0);
#endif

        {   // regardless of whether we are 32 or 64 bit, remove all 32bit and 64bit registrations.
            HKEY Key32Clsid;
            if (ERROR_SUCCESS == ::RegOpenKeyExW(ClassesRoot, L"CLSID", 0, 
                DELETE | KEY_ENUMERATE_SUB_KEYS |KEY_SET_VALUE | KEY_QUERY_VALUE | KEY_WOW64_32KEY, &Key32Clsid))
            {
                ::RegDeleteTreeW(Key32Clsid,clsid);
                ::RegCloseKey(Key32Clsid);
            }
             HKEY Key64Clsid;
            if (ERROR_SUCCESS == ::RegOpenKeyExW(ClassesRoot, L"CLSID", 0, 
                DELETE | KEY_ENUMERATE_SUB_KEYS |KEY_SET_VALUE | KEY_QUERY_VALUE | KEY_WOW64_64KEY, &Key64Clsid))
            {
                ::RegDeleteTreeW(Key64Clsid,clsid);
                ::RegCloseKey(Key64Clsid);
            }
       }

        fns.resize(finalbSlash+1);
        fns += L"DigiRite.exe";

        HKEY hkclsid;
        auto res = ::RegCreateKeyExW(ClassesRoot, 
            ClsidStr,
            0,
            0,
            0,
            KEY_READ|KEY_WRITE,
            0,
            &hkclsid,
            0
        );
        if (ERROR_SUCCESS == res)
        {
            HKEY hkpid;
            auto res2 = ::RegCreateKeyExW(ClassesRoot,
                L"WriteLog.Ft8Auto\\CLSID",
                0,
                0,
                0,
                KEY_READ | KEY_WRITE,
                0,
                &hkpid,
                0
            );
            if (ERROR_SUCCESS == res2)
            {
                auto sv1 = ::RegSetValueW(hkpid, 0, REG_SZ, clsid, sizeof(clsid));
                HKEY hkdpi;
                auto res3 = ::RegCreateKeyExW(LocalMachine,
                    L"Software\\W5XD\\WriteLog\\DigitalProgIds",
                    0,
                    0,
                    0,
                    KEY_READ | KEY_WRITE,
                    0,
                    &hkdpi,
                    0
                );
                if (ERROR_SUCCESS == res3)
                {
                    static const wchar_t PROGID[] = L"WriteLog.Ft8Auto";
                    ::RegSetKeyValueW(hkdpi, 0, L"DigiRite", REG_SZ, PROGID, sizeof(PROGID));

                    ::RegSetValueW(hkclsid, 0, REG_SZ, PROGID, sizeof(PROGID));
                    ::RegSetValueW(hkclsid, L"ProgId", REG_SZ, PROGID, sizeof(PROGID));
                    ::RegSetValueW(hkclsid, L"LocalServer32", REG_SZ, fns.c_str(), (DWORD)(fns.size() * sizeof(wchar_t)));
                    ::RegSetValueW(hkclsid, L"Implemented Categories\\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}", REG_SZ, L"", 0);
                    ::RegCloseKey(hkdpi);
                }
                ::RegCloseKey(hkpid);
            }
            else
                ::MessageBoxW(0, L"Can't open PROGID", L"DigiRite Register", 0);
            ::RegCloseKey(hkclsid);
        }
        else
            ::MessageBoxW(0, L"Cannot open registry CLSID key", L"DigiRite Register", 0);

#if defined(_DEBUG)
        ::RegCloseKey(ClassesRoot);
        ::RegCloseKey(LocalMachine);
#endif

    }
    else
        ::MessageBoxW(0, L"Error getting module name", L"DigiRite Register", 0);
}

