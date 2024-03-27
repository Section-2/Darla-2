document.addEventListener('DOMContentLoaded', () => {
    function updateOrder() {
        const itemOrder = Array.from(document.querySelectorAll('.container .box')).map((item, index) => ({
            id: item.dataset.teamnumber, // Assuming you have data-teamnumber as an identifier
            position: index
        }));
        localStorage.setItem('itemOrder', JSON.stringify(itemOrder));
    }

    function loadOrder() {
        const itemOrderStr = localStorage.getItem('itemOrder');
        if (!itemOrderStr) return;
        const itemOrder = JSON.parse(itemOrderStr);
        const container = document.querySelector('.container');

        // Create a map of id to box elements
        const itemsMap = {};
        document.querySelectorAll('.container .box').forEach(item => {
            itemsMap[item.dataset.teamnumber] = item;
        });

        // Sort the container's children according to the saved order
        itemOrder.forEach(({ id }) => {
            if (itemsMap[id]) {
                container.appendChild(itemsMap[id]);
            }
        });
    }

    let dragSrcEl = null;

    function handleDragStart(e) {
        dragSrcEl = this;
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/html', this.outerHTML);
    }

    function handleDragOver(e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
        return false;
    }

    function handleDragEnter(e) {
        this.classList.add('over');
    }

    function handleDragLeave(e) {
        this.classList.remove('over');
    }

    function handleDrop(e) {
        e.stopPropagation();
        e.preventDefault();

        if (dragSrcEl !== this) {
            dragSrcEl.parentNode.removeChild(dragSrcEl);
            const dropHTML = e.dataTransfer.getData('text/html');
            this.insertAdjacentHTML('beforebegin',dropHTML);
            const dropElem = this.previousSibling;
            addDnDEvents(dropElem);

            // Clear the dragged content from the source element
            e.dataTransfer.clearData();

            // Update the order in local storage after drop
            updateOrder();
        }

        return false;
    }

    function handleDragEnd(e) {
        this.style.opacity = '1';
        items.forEach(function (item) {
            item.classList.remove('over');
        });
    }

    function addDnDEvents(elem) {
        elem.addEventListener('dragstart', handleDragStart, false);
        elem.addEventListener('dragenter', handleDragEnter, false);
        elem.addEventListener('dragover', handleDragOver, false);
        elem.addEventListener('dragleave', handleDragLeave, false);
        elem.addEventListener('drop', handleDrop, false);
        elem.addEventListener('dragend', handleDragEnd, false);
    }

    // Load the order as soon as possible
    loadOrder();

    let items = document.querySelectorAll('.container .box');
    items.forEach(addDnDEvents);
});
