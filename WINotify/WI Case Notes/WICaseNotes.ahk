#NoEnv ; For performance and compatibility with future AutoHotkey releases.
#SingleInstance  ; Allow only one instance of this script to be running.
SendMode Input  ; For superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.
#InstallKeybdHook	;Consumes more memory, but should help with sticking hotkeys

IfExist, %A_ScriptDir%\cases.csv
{
	IfExist, %A_ScriptDir%\message.txt
	{
		MsgBox, 4, , Start? (Press YES or NO)
		IfMsgBox No
			ExitApp
		Goto LetsDoThis
	}
	else
	{
	FileAppend, , %A_ScriptDir%\message.txt
	MsgBox, Added message.txt.  Please edit approperately.
	ExitApp
	}
}
else
{
	FileAppend, , %A_ScriptDir%\cases.csv
	FileAppend, , %A_ScriptDir%\message.txt
	MsgBox, Added cases.csv and message.txt.  Please edit approperately.
	ExitApp
}


LetsDoThis:
{
FileCopy, %A_ScriptDir%\cases.csv, %A_ScriptDir%\bak\%A_YYYY%-%A_MM%-%A_DD%-cases.csv
FileRead, Mess, %A_ScriptDir%\message.txt

Loop, read, %A_ScriptDir%\cases.csv
{
    LineNumber = %A_Index%
    Loop, parse, A_LoopReadLine, CSV
    {
		RowNumber = %A_Index%
		AddToCase(A_LoopField, Mess)        
    }
}
FileDelete, %A_ScriptDir%\cases.csv
Sleep 300
FileAppend, , %A_ScriptDir%\cases.csv
}
FindCase(CaseNum)
{
	OpenClarify()
	ATC_CaseWinTitle := "Amdocs CRM - ClearSupport - [ Case " . CaseNum
	IfWinNotExist %ATC_CaseWinTitle%  ;if the case is already open and active, we can skip this
	{
		Clipboard := CaseNum
		Send ^1^i
		WinWait Find Case By ID,, 5
		IfWinActive Find Case By ID
			Send ^v{ENTER}						;CTRL+1 switches to Case Mode, CTRL + i opens case search
		else
		{
			TrayTip Simplify, Unable to open Clarify Find Case window, 10, 2
			;todo add error notification
			exit
		}
		
		AdPriority = The current status of the product linked to this case is Adv Priority

		WinWait ,,%AdPriority%, 3
		IfWinActive ,, %AdPriority%
		{
			WinWaitClose ,, %AdPriority% ,3
			IfWinActive ,, %AdPriority%
				Send {ENTER}
		}
	}
	return
}
AddToCase(CaseNum, Message)
{
	OpenClarify()	
		FindCase(CaseNum)
	
	Sleep 200
	
		IfWinActive ahk_class #32770
			WinWaitClose ahk_class #32770
			
	Sleep 200
	
	Send {CTRLDOWN}o{CTRLUP}						;Open Notes
	ATC_CaseWinTitle := "Amdocs CRM - ClearSupport - [" . CaseNum . ":Notes"
	WinWait %ATC_CaseWinTitle%,,10
	IfWinActive Amdocs CRM - ClearSupport - [[READ] Case
	{
		TrayTip Simplify, Notes not added`nCase probably closed, 10, 2
			;todo -- add a note specifying there was an error.
	}	
	IfWinNotActive %CaseWinTitle%
	{
		TrayTip Simplify, Notes not added`nCould not open notes for case, 10, 2
		;todo -- add a note specifying there was an error.
	}	
	
	Control ChooseString,Notes, ComboBox6		;Select Action Type Dropdown and set it to Internal Email
	Send {Enter}
	ControlSetText, RichEdit20A1, %Message%
	Send !d												;Click Done, prompts user to save notes											
	Sleep 300
	Send {Enter}
	; ----- Begin move case to first wipbin with X in it.
	;Sleep 500	
	;Send ^m
	;Sleep 500
	;Send X
	;Sleep 300
	;Send {Enter}
	; ----- End move case to first wipbin with X in it. 
	Sleep 800
	Send ^w 								; Closes Case Window
	Sleep 100
	Send ^w
	Sleep 200
}
OpenClarify()
{
	IfWinNotExist Amdocs
	{	
		RegRead, ClarifyDir, HKEY_LOCAL_MACHINE, SOFTWARE\Clarify\ClarifyClient, InstallDir		; Find out where Clarify is installed 
		InstallExe := ClarifyDir . "\ClarifyClient\clarify.exe"
		ClarifyWorkingDir := ClarifyDir . "\ClarifyClient"
		run %InstallExe%, %ClarifyWorkingDir%
		WinWait Amdocs CRM -
		Sleep 2000
		WinActivate Amdocs
	}
	else
		WinActivate Amdocs
	return
}
