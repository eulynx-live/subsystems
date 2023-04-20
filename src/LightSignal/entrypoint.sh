#!/bin/bash


if [[ -z "${LOCAL_IDS}" ]]; then
  echo "LOCAL_IDS was not set. All light signals found in Interlocking.exml will be simulated!"
else
   IFS=',' read -r -a LOCAL_IDS <<< "$LOCAL_IDS"
fi

if [[ -z "${REMOTE_ID}" ]]; then
  REMOTE_ID="INTERLOCKING"
  echo "REMOTE_ID was not set, default to $REMOTE_ID"
fi

if [[ -z "${LOCAL_RASTA_ID}" ]]; then
  LOCAL_RASTA_ID="9876543"
  echo "LOCAL_RASTA_ID was not set, default to $LOCAL_RASTA_ID"
fi

if [[ -z "${REMOTE_ENDPOINT}" ]]; then
  REMOTE_ENDPOINT="http://interlocking:5100"
  echo "REMOTE_ENDPOINT was not set, default to $REMOTE_ENDPOINT"
fi

# Find all sig to rsm references
refs=$( xq -x "//generic:ownsTrackAsset[@xsi:type = 'syslab:RastaSignal']/sig:refersToRsmSignal/@ref"  /config/Interlocking.exml)
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
   if (printf '%s\0' "${LOCAL_IDS[@]}" | grep -Fxzq -- "$name") || [[ -z "${LOCAL_IDS}" ]] ;
   then 
      echo -n "LightSignals__${COUNTER}__id=$name "
      echo -n "LightSignals__${COUNTER}__type=$f2 "
      COUNTER=$[$COUNTER +1]
   fi
done 3< <(echo "$refs") 4< <(echo "$types")
)
echo "Found signals: $args"
eval $(echo "$args dotnet LightSignal.dll --remote-id $REMOTE_ID --local-rasta-id $LOCAL_RASTA_ID --remote-endpoint $REMOTE_ENDPOINT" )
