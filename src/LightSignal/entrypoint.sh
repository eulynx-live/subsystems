#!/bin/bash
echo LOCAL_ID $LOCAL_ID
id=$(cat /config/Interlocking.exml | sed -e 's/<?xml version="1.0" encoding="utf-16"?>/<?xml version="1.0" encoding="utf-8"?>/g' | xq -x "//db:longNameLayoutPlan[.=\"$LOCAL_ID\"]/../../../rsmCommon:id")
echo xq found id: $id
export SIGNAL_TYPE=$(cat /config/Interlocking.exml | sed -e 's/<?xml version="1.0" encoding="utf-16"?>/<?xml version="1.0" encoding="utf-8"?>/g' |  xq -x "//rsmCommon:id[.=\"$id\"]//following-sibling::db:isOfSignalType")
echo xq found SIGNAL_TYPE: $SIGNAL_TYPE

dotnet LightSignal.dll --signal_type $SIGNAL_TYPE "$@"
