#$csvdata  = Get-Content C:\Users\Wornox\Desktop\Test.csv
#Set-Clipboard $csvdata 

$location = Get-Location

#Foreach-Object{
    
    $folder = Get-ChildItem $location

#}

#Foreach-Object{
    
    $files = Get-ChildItem $folder 

#}

$csvdataChicken = ""
$csvdataCow = ""
$csvdataDog = ""
$csvdataFox = ""
$csvdataLion = ""
$csvdataPig = ""

foreach ($f in $files){
    
    if($f.Name -match  'SimulationStatistics_Chicken'){
        $dataChicken = Get-Content $f.FullName #| Where-Object {$f.Name -match  'Chicken'}
        $csvdataChickenHeader = $dataChicken | Select-Object -First 1 #fejléc sor 
        $csvdataChicken += $dataChicken | Select-Object -Last 1 #utolsó sor
        $csvdataChicken += "`n"

    }

    if($f.Name -match  'SimulationStatistics_Cow'){
        $dataCow = Get-Content $f.FullName #| Where-Object {$f.Name -match  'Chicken'}
        $csvdataCowHeader = $dataCow | Select-Object -First 1 #fejléc sor 
        $csvdataCow += $dataCow | Select-Object -Last 1 #utolsó sor
        $csvdataCow += "`n"

    }

    if($f.Name -match  'SimulationStatistics_Dog'){
        $dataDog = Get-Content $f.FullName #| Where-Object {$f.Name -match  'Chicken'}
        $csvdataDogHeader = $dataDog | Select-Object -First 1 #fejléc sor 
        $csvdataDog += $dataDog | Select-Object -Last 1 #utolsó sor
        $csvdataDog += "`n"
    }

    if($f.Name -match  'SimulationStatistics_Fox'){
        $dataFox = Get-Content $f.FullName #| Where-Object {$f.Name -match  'Chicken'}
        $csvdataFoxHeader = $dataFox | Select-Object -First 1 #fejléc sor 
        $csvdataFox += $dataFox | Select-Object -Last 1 #utolsó sor
        $csvdataFox += "`n"
    }

    if($f.Name -match  'SimulationStatistics_Lion'){
        $dataLion = Get-Content $f.FullName #| Where-Object {$f.Name -match  'Chicken'}
        $csvdataLionHeader = $dataLion | Select-Object -First 1 #fejléc sor 
        $csvdataLion += $dataLion | Select-Object -Last 1 #utolsó sor
        $csvdataLion += "`n"
    }

    if($f.Name -match  'SimulationStatistics_Pig'){
        $dataPig = Get-Content $f.FullName #| Where-Object {$f.Name -match  'Chicken'}
        $csvdataPigHeader = $dataPig | Select-Object -First 1 #fejléc sor 
        $csvdataPig += $dataPig | Select-Object -Last 1 #utolsó sor
        $csvdataPig += "`n"
    }
}

#Write-Output $csvdataChicken
#Write-Output $csvdataDog

#Set-Clipboard $csvdata

$csvOUTChicken = $csvdataChickenHeader + "`n" + $csvdataChicken
$csvOUTCow = $csvdataCowHeader + "`n" + $csvdataCow
$csvOUTDog = $csvdataDogHeader + "`n" + $csvdataDog
$csvOUTFox = $csvdataFoxHeader + "`n" + $csvdataFox
$csvOUTLion = $csvdataLionHeader + "`n" + $csvdataLion
$csvOUTPig = $csvdataPigHeader + "`n" + $csvdataPig

#Write-Output $csvdataChickenHeader


Set-Content -Path '.\Chicken.csv' -Value $csvOUTChicken
Set-Content -Path '.\Cow.csv' -Value $csvOUTCow
Set-Content -Path '.\Dog.csv' -Value $csvOUTDog
Set-Content -Path '.\Fox.csv' -Value $csvOUTFox
Set-Content -Path '.\Lion.csv' -Value $csvOUTLion
Set-Content -Path '.\Pig.csv' -Value $csvOUTPig


#$csvdata  = Get-Content $file2
#    $csvdata = $csvdata | Select-Object -Last 1
#    Write-Output $csvdata
#
    #$data = Get-ChildItem $file

#Set-Clipboard $csvdata 
