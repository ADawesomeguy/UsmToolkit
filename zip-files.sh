FOLDERS=UsmToolkit/bin/Release/netcoreapp3.1/*/

for f in $FOLDERS
do
    zip -r "${f%/*}.zip" $f
done
