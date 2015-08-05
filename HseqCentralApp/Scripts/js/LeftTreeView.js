
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

    //console.log(recordTypeCheckState);
    //console.log(responsibleAreaCheckState);
    //console.log(coordinatorsCheckState);

    MainContentCallbackPanel.PerformCallback();

}


function OnBeginCallback(s, e) {

    e.customArgs["recordTypeCheckState"] = recordTypeCheckState;
    e.customArgs["responsibleAreaCheckState"] = responsibleAreaCheckState;
    e.customArgs["coordinatorsCheckState"] = coordinatorsCheckState;

    if (recordTypeCheckedNodes !== null) {
        e.customArgs["recordTypeCheckedNodes"] = recordTypeCheckedNodes.join(",");
    }
    if (responsibleAreaCheckedNodes !== null) {
        e.customArgs["responsibleAreaCheckedNodes"] = responsibleAreaCheckedNodes.join(",");
    }
    if (responsibleAreaCheckedNodes !== null) {
        e.customArgs["coordinatorsCheckedNodes"] = coordinatorsCheckedNodes.join(",");
    }

    e.customArgs["currentActiveTabIndex"] = MainContentTabPanel.GetActiveTabIndex();

    //////////////////////////////////

    e.customArgs["currentActiveView"] = currentActiveView;
    e.customArgs["recordId"] = recordId;
    e.customArgs["newcomment"] = commentAddNewMeno.lastChangedValue;

    if (editbtnclicked) {

        e.customArgs["edit"] = true;
    } else if (newbtnclicked) {

        e.customArgs["new"] = true;
    }

    newbtnclicked = false;
    editbtnclicked = false;
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var currentActiveView;
var currentActiveViewObj;
 var recordId;

function ncrFocusChanged(s, e) {

    //LinkedItemsPanel.SetContentHtml(s.name + " - " + s.GetRowKey(NcrGridView.GetFocusedRowIndex()));
    gridViewItemFocusChanged(s, e);
}

function carFocusChanged(s, e) {
    gridViewItemFocusChanged(s, e);
}

function parFocusChanged(s, e) {
    gridViewItemFocusChanged(s, e);
}

function fisFocusChanged(s, e) {
    gridViewItemFocusChanged(s, e);
}

function allItemsFocusChanged(s, e) {
    gridViewItemFocusChanged(s, e);
}

function taskFocusChanged(s, e) {
    gridViewItemFocusChanged(s, e);
}

function approvalFocusChanged(s, e) {
    gridViewItemFocusChanged(s, e);
}
        
function gridViewItemFocusChanged(obj_s, obj_e) {
    currentActiveView = obj_s.name;
    recordId = obj_s.GetRowKey(obj_s.GetFocusedRowIndex());
    //MainContentCallbackPanel.PerformCallback();
    LinkedRecordsPanel.PerformCallback();
    CommentsPanel.PerformCallback();
}

function OnLinkedRecordsBeginCallback(s, e) {

    computeCurrentRecord();

    e.customArgs["currentActiveView"]= currentActiveView;
    e.customArgs["recordId"]= recordId;
}

function OnLinkedRecordsEndCallback(s, e) {
}

function OnCommentsBeginCallback(s, e) {

    computeCurrentRecord();

    e.customArgs["currentActiveView"]= currentActiveView;
    e.customArgs["recordId"]= recordId;
    e.customArgs["newcomment"] = commentAddNewMeno.lastChangedValue;
         
}

function OnCommentsEndCallback(s, e) { 
    commentAddNewMeno.lastChangedValue = "";
}

function AddNewComment(s, e) {

    console.log(commentAddNewMeno.lastChangedValue);

    CommentsPanel.PerformCallback();
}

function InitComment() {
    commentAddNewMeno.SetVisible(false);
    commentSaveNewBtn.SetVisible(false);
}

function OnRightContentPanelBeginCallback(s, e) { 
    e.customArgs["currentActiveTabIndex"]= MainContentTabPanel.GetActiveTabIndex();
}

function MainContentTabPanelTabChanged(s, e) {
    //RightContentCallbackPanel.PerformCallback();
    console.log(s.GetActiveTabIndex());

    computeCurrentRecord();

    console.log(currentActiveView + " - " +recordId);

    CommentsPanel.PerformCallback();
    LinkedRecordsPanel.PerformCallback();
}

function OnMainContentTabPanelBeginCallback(s, e) {}

function OnMainContentTabPanelEndCallback(s, e) { }

function computeCurrentRecord(){
    
    if (MainContentTabPanel.GetActiveTabIndex() === 0) {
        currentActiveViewObj = NcrGridView;
        currentActiveView = NcrGridView.name;
        recordId = NcrGridView.GetRowKey(NcrGridView.GetFocusedRowIndex());
        }
        else if (MainContentTabPanel.GetActiveTabIndex() === 1) {
        currentActiveViewObj = CarGridView;
        currentActiveView = CarGridView.name;
        recordId = CarGridView.GetRowKey(CarGridView.GetFocusedRowIndex());
        }
        else if (MainContentTabPanel.GetActiveTabIndex() === 2) {
        currentActiveViewObj = ParGridView;
        currentActiveView = ParGridView.name;
        recordId = ParGridView.GetRowKey(ParGridView.GetFocusedRowIndex());
        }
        else if (MainContentTabPanel.GetActiveTabIndex() === 3) {
        currentActiveViewObj = FisGridView;
        currentActiveView = FisGridView.name;
        recordId = FisGridView.GetRowKey(FisGridView.GetFocusedRowIndex());
        }
        else if(MainContentTabPanel.GetActiveTabIndex() === 4) {
        currentActiveViewObj = TaskGridViewPartial;
        currentActiveView = TaskGridViewPartial.name;
        recordId = TaskGridViewPartial.GetRowKey(TaskGridViewPartial.GetFocusedRowIndex());
        }
        else if (MainContentTabPanel.GetActiveTabIndex() === 5) {
        currentActiveViewObj = ApprovalGridViewPartial;
        currentActiveView = ApprovalGridViewPartial.name;
        recordId = ApprovalGridViewPartial.GetRowKey(ApprovalGridViewPartial.GetFocusedRowIndex());
        }
    else if (MainContentTabPanel.GetActiveTabIndex() === 6) {
        currentActiveViewObj = AllItemsGridView;
        currentActiveView = AllItemsGridView.name;
        recordId = AllItemsGridView.GetRowKey(AllItemsGridView.GetFocusedRowIndex());
    }
    
    var results =[{currentActiveViewObj: currentActiveViewObj, currentActiveView: currentActiveView, recordId: recordId}];

    //alert(list[1].currentActiveView);

    return results;
}

MVCxClientGlobalEvents.AddControlsInitializedEventHandler(function (s, e) {
    if (e.isCallback === true) {
        //console.log(s);
    //console.log(e);
    }
});


//function AddNewComment(s, e) {
//        $.ajax({
//            type: "POST",
//            url: '@Url.Action("AddNewComment", "Controllers/Navigation")',
//                        data: { selectedMenuItemName: s.name },
//            beforeSend: function() {
//                //loadingPanel.Show();
//            },
//            success: function(response) {
//               // $("#container").html(response);
//                //loadingPanel.Hide();
//            }
//        });
//    }


//function OnCommandExecuted(s, e) {
//    $.post("/Ncrs/NcrGridViewUpdate1?ParamValue1=" +NcrGridView.GetRowKey(NcrGridView.GetFocusedRowIndex()), function (data) {
//        alert(data);
//    }, function (err) {
//        alert(err);
//    });
//    MainContentCallbackPanel.PerformCallback();
//    }


//////////////////////////////////////////////////////////////////////////////////////////

//Ncr Grid View
function EditRow(obj) {
    var focusedRowIndex = obj.GetFocusedRowIndex();
//NcrGridView.GetRowValues(focusedRowIndex, 'HseqRecordID', OnGetRowValues);
    obj.StartEditRow(focusedRowIndex);
}

function DeleteRow(obj) {
    var focusedRowIndex = obj.GetFocusedRowIndex();
    obj.DeleteRow(focusedRowIndex);
}


//Ncr Grid View

function GridNewRow(s, e) {

    var results = computeCurrentRecord();
    console.log(results);
    var activeViewObj = results[0].currentActiveViewObj;

    //    activeViewObj.AddNewRow();

    console.log(s.name);

    var focusedRowIndex = activeViewObj.GetFocusedRowIndex();
    newbtnclicked = true;
    MainContentCallbackPanel.PerformCallback();
    }

var editbtnclicked = false;
var newbtnclicked = false;

function GridEditRow(s,e) {
    var results = computeCurrentRecord();
    var activeViewObj = results[0].currentActiveViewObj;

    console.log(s.name);

    var focusedRowIndex = activeViewObj.GetFocusedRowIndex();

    editbtnclicked = true;
    

        //activeViewObj.StartEditRow(focusedRowIndex);

    MainContentCallbackPanel.PerformCallback();
}

function GridDeleteRow(s,e) {
    var results = computeCurrentRecord();
    var activeViewObj = results[0].currentActiveViewObj;

    var focusedRowIndex = activeViewObj.GetFocusedRowIndex();
    activeViewObj.DeleteRow(focusedRowIndex);
}

function OnClick(s, e) {
        var actionParams = $("form").attr("action").split("?OutputFormat=");
        actionParams[1] = s.GetMainElement().getAttribute("OutputFormatAttribute");

        $("form").attr( { action:actionParams.join("?OutputFormat=")} );
}


//function OnNcrEditClick(s, e) {
//    $.ajax({
//        type: "POST",
//        url: "@Url.Action("NcrGridViewUpdate","Navigation")",
//        success: function(response) {
//            $("#container").html(response);
//        }
//    });
//}


function PostFailure() {};

function PostSuccess(response) {

    // $(MainPaneSplitter_1_CC).html(response);
        $(MainPaneSplitterContainer).html(response);
        //$(MainContentPane).html(response);
        
};

function PostOnComplete() {};


//function ncrEditCancel() {
//        $(MainPaneSplitterContainer).html($("_MainContentCallbackPanel"));

//};

$(function() {
    $('editCancelButton').click(function() {
        $.ajax({
            //url: $(this).data('url'),
            type: 'GET',
            cache: false,
            success: function(result) {
                $(MainPaneSplitterContainer).html($("_MainContentCallbackPanel"));
            }
        });
        return false;
    });
});


$(function() {
    $('carCancelButton').click(function() {
        $.ajax({
            //url: $(this).data('url'),
            type: 'GET',
            cache: false,
            success: function(result) {
                $(MainPaneSplitterContainer).html($("_MainContentCallbackPanel"));
            }
        });
        return false;
    });
});

$(function () {
    $('parCancelButton').click(function () {
        $.ajax({
            //url: $(this).data('url'),
            type: 'GET',
            cache: false,
            success: function (result) {
                $(MainPaneSplitterContainer).html($("_MainContentCallbackPanel"));
            }
        });
        return false;
    });
});

$(function () {
    $('fisCancelButton').click(function () {
        $.ajax({
            //url: $(this).data('url'),
            type: 'GET',
            cache: false,
            success: function (result) {
                $(MainPaneSplitterContainer).html($("_MainContentCallbackPanel"));
            }
        });
        return false;
    });
});