function chickenscratch_treetable_updateCollapsedBy(collapsing, elmt, targetPath) {
    let attr = elmt.getAttribute("data-chickenscratch-treetable-collapsedby")
    if (collapsing && attr == null) {
        elmt.setAttribute("data-chickenscratch-treetable-collapsedby", `|${targetPath}|`);
    } else if (collapsing && !attr.includes(`|${targetPath}|`)) {
        elmt.setAttribute("data-chickenscratch-treetable-collapsedby", `${attr}|${targetPath}|`);
    } else if (!collapsing && attr.includes(`|${targetPath}|`)) {
        let newAttr = attr.replace(`|${targetPath}|`, "");
        if (newAttr.length === 0) {
            elmt.removeAttribute("data-chickenscratch-treetable-collapsedby");
        } else {
            elmt.setAttribute("data-chickenscratch-treetable-collapsedby", newAttr);
        }
    }
}

function chickenscratch_treetable_toggle(treeId, targetPath) {
    let tree = document.getElementById(treeId);
    let toggleRow = tree.querySelector(`[data-chickenscratch-treetable-path='${targetPath}']`);
    let collapsing = toggleRow.classList.contains("chickenscratch_treetable_expanded");
    tree.querySelectorAll("tr").forEach(function (elmt) {
        let path = elmt.getAttribute("data-chickenscratch-treetable-path");                
        if (path !== null && path.includes(targetPath + "/")) {
            chickenscratch_treetable_updateCollapsedBy(collapsing, elmt, targetPath);
        }
    })
    
    if (collapsing) {
        toggleRow.classList.remove("chickenscratch_treetable_expanded");
        toggleRow.classList.add("chickenscratch_treetable_collapsed");
    } else {
        toggleRow.classList.add("chickenscratch_treetable_expanded");
        toggleRow.classList.remove("chickenscratch_treetable_collapsed");
    }
}