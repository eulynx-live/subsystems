#!/bin/bash

# Find all sig to rsm references
refs=$( xq -x "//generic:ownsTrackAsset[@xsi:type = 'sig:Signal']/sig:refersToRsmSignal/@ref"  /config/Interlocking.exml)
# Find all types
types=$( xq -x "//sig:hasProperty/db:isOfSignalTypeType" /config/Interlocking.exml )

# For each reference and type print signal name and type into stdout
COUNTER=0
args=$(
while true
do
   read -r f1 <&3 || break
   read -r f2 <&4 || break
   name=$( xq -x "//generic:ownsSignal/rsmCommon:id[.=\"$f1\"]/following-sibling::rsmCommon:name" /config/Interlocking.exml )
   echo -n "LightSignals__${COUNTER}__id=$name "
   echo -n "LightSignals__${COUNTER}__type=$f2 "
   COUNTER=$[$COUNTER +1]
done 3< <(echo "$refs") 4< <(echo "$types")
)
echo "Found signals: $args"
eval $(echo "$args dotnet LightSignal.dll --remote-id INTERLOCKING --local-rasta-id "9876543" --remote-endpoint http://interlocking:5100" )
