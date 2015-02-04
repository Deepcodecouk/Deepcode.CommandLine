#################################
#
# Output utils
#
#################################
Function global:Write-Skip()
{
	Param(
		[string] $Text
		);

	Write-Host "** SKIP: $Text" -ForegroundColor Blue
}

Function global:Write-Section()
{
	Param(
		[string] $Text
		);

	Write-Host "***********************************************************************" -ForegroundColor Yellow
	Write-Host "* $Text" -ForegroundColor Yellow
	Write-Host "***********************************************************************" -ForegroundColor Yellow
}

Function global:Write-Error()
{
	Param(
		[string] $Text
		);

	Write-Host "***********************************************************************" -ForegroundColor Red
	Write-Host "* $Text" -ForegroundColor Red
	Write-Host "***********************************************************************" -ForegroundColor Red
}

Function global:Write-Banner()
{
	Param(
		[string] $Title,
		[string] $Body,
		[string] $Colour = "Green"
		);
	Write-Host "`r`n`n"
	Write-Host "***********************************************************************" -ForegroundColor $Colour
	if( $Title -ne $null )
	{
		Write-Host "*" -ForegroundColor $Colour
		Write-Host "* $Title" -ForegroundColor $Colour
		Write-Host "*" -ForegroundColor $Colour
		Write-Host "-----------------------------------------------------------------------" -ForegroundColor $Colour
	}

	$newBody = $Body -replace "\\n", "`r`n* "
	Write-Host "* $newBody" -ForegroundColor $Colour
	Write-Host "***********************************************************************`r`n`n`n" -ForegroundColor $Colour
}
