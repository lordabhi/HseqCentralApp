﻿
//var employeeId = null;
var recordTypeCheckedNodes = null;
var responsibleAreaCheckedNodes = null;
var coordinatorsCheckedNodes = null;

var recordTypeCheckState = null;
var responsibleAreaCheckState = null;
var coordinatorsCheckState = null;


function OnRecordTypeChanged(s, e) {
        leftHandNavigationFilterChanged(s,e);
    }

    function OnResponsibleAreaChanged(s, e) {
        leftHandNavigationFilterChanged(s, e);
    }
    
    function OnCoordinatorChanged(s, e) {
        leftHandNavigationFilterChanged(s, e);
    }

    function InitDisplay() {
        leftHandNavigationFilterChanged(null, null);

    }

    function leftHandNavigationFilterChanged(s, e) {

        var parentNodeIndex = 0;

        console.log("leftHandNavigationFilterChanged      " + s.name);

        //Record Type Checked Nodes

        recordTypeCheckState = RecordTypeTreeViewPanel.GetNodeState(RecordTypeTreeViewPanel.GetNode(parentNodeIndex));

        recordTypeCheckedNodes = new Array();

//        console.log("Parent Node Checked Status: " + RecordTypeTreeViewPanel.GetNode(parentNodeIndex).GetChecked());

        for (var i = 0; i < RecordTypeTreeViewPanel.GetNode(parentNodeIndex).GetNodeCount() ; i++) {

            if (RecordTypeTreeViewPanel.GetNode(parentNodeIndex).GetNode(i).GetChecked()) {
                //recordTypeCheckedNodes.push(RecordTypeTreeViewPanel.GetNode(i).GetText());
                recordTypeCheckedNodes.push(RecordTypeTreeViewPanel.GetNode(parentNodeIndex).GetNode(i).name);
            }
        }

        //Responsible Area Checked Nodes

        responsibleAreaCheckState = ResponsibleAreaTreeViewPanel.GetNodeState(ResponsibleAreaTreeViewPanel.GetNode(parentNodeIndex));

        responsibleAreaCheckedNodes = new Array();
        for (var i = 0; i < ResponsibleAreaTreeViewPanel.GetNode(parentNodeIndex).GetNodeCount() ; i++) {
            if (ResponsibleAreaTreeViewPanel.GetNode(parentNodeIndex).GetNode(i).GetChecked()) {
                //responsibleAreaCheckedNodes.push(ResponsibleAreaTreeViewPanel.GetNode(i).GetText());
                responsibleAreaCheckedNodes.push(ResponsibleAreaTreeViewPanel.GetNode(parentNodeIndex).GetNode(i).name);
            }
        }

        //Coordinators Checked Node

        coordinatorsCheckState = CoordinatorTreeViewPanel.GetNodeState(CoordinatorTreeViewPanel.GetNode(parentNodeIndex));

        coordinatorsCheckedNodes = new Array();
        for (var i = 0; i < CoordinatorTreeViewPanel.GetNode(parentNodeIndex).GetNodeCount() ; i++) {
            if (CoordinatorTreeViewPanel.GetNode(parentNodeIndex).GetNode(i).GetChecked()) {
                //coordinatorsCheckedNodes.push(CoordinatorTreeViewPanel.GetNode(i).GetText());
                coordinatorsCheckedNodes.push(CoordinatorTreeViewPanel.GetNode(parentNodeIndex).GetNode(i).name);
            }
        }

        console.log(recordTypeCheckState);
        console.log(responsibleAreaCheckState);
        console.log(coordinatorsCheckState);

        console.log("Entering PerformCallback")
        MainContentCallbackPanel.PerformCallback();
        console.log("Exiting PerformCallback")
    }


function OnBeginCallback(s, e) {

    e.customArgs["recordTypeCheckState"] = recordTypeCheckState;
    e.customArgs["responsibleAreaCheckState"] = responsibleAreaCheckState;
    e.customArgs["coordinatorsCheckState"] = coordinatorsCheckState;


    if (recordTypeCheckedNodes !== null) {
        e.customArgs["recordTypeCheckedNodes"] = recordTypeCheckedNodes.join(",");
    }
    if (responsibleAreaCheckedNodes!==null) {
        e.customArgs["responsibleAreaCheckedNodes"] = responsibleAreaCheckedNodes.join(",");
    }
    if (responsibleAreaCheckedNodes !== null) {
        e.customArgs["coordinatorsCheckedNodes"] = coordinatorsCheckedNodes.join(",");
    }

    e.customArgs["currentActiveTabIndex"] = MainContentTabPanel.GetActiveTabIndex();
    
}

//////////////////////////////////////////////////////////////////////////////////////////

//Ncr Grid View
function EditRow(obj) {
    var focusedRowIndex = obj.GetFocusedRowIndex();
    obj.StartEditRow(focusedRowIndex);
}

function DeleteRow(obj) {
    var focusedRowIndex = obj.GetFocusedRowIndex();
    obj.DeleteRow(focusedRowIndex);
}
