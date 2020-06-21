!macro customUnInstall
  MessageBox MB_YESNO "Delete app config folder?" \
    /SD IDNO IDNO Skipped IDYES Accepted
    
  Accepted:
    SetShellVarContext current
    RMDir /r "$APPDATA\spirittime"
    RMDir /r "LOCALAPPDATA\spirittime-updater"
    
    Goto done
  Skipped:
    Goto done
  done:
!macroend