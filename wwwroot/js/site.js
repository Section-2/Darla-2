document.addEventListener('DOMContentLoaded', () => {
    function updateOrder() {
        // Create an array that represents the current order of items
        const itemOrder = Array.from(document.querySelectorAll('.ranking-list .box')).map((item, index) => ({
            id: item.dataset.teamnumber,
            position: index
        }));
        // Store the item order in localStorage
        localStorage.setItem('itemOrder', JSON.stringify(itemOrder));
    }

    function loadOrder() {
        // Retrieve the stored item order from localStorage
        const itemOrderStr = localStorage.getItem('itemOrder');
        if (!itemOrderStr) return;
        const itemOrder = JSON.parse(itemOrderStr);
        const rankingList = document.querySelector('.ranking-list');

        // Create a map of id to box elements
        const itemsMap = {};
        document.querySelectorAll('.ranking-list .box').forEach(item => {
            itemsMap[item.dataset.teamnumber] = item;
        });

        // Sort the rankingList's children according to the saved order
        itemOrder.forEach(({ id }) => {
            if (itemsMap[id]) {
                rankingList.appendChild(itemsMap[id]);
            }
        });
    }

    function handleDragStart(e) {
        // Set the opacity of the element being dragged
        this.style.opacity = '0.4';

        // Store the element being dragged
        dragSrcEl = this;

        // Set the data transfer object
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/html', this.outerHTML);
    }

    function handleDragOver(e) {
        // Prevent default to allow drop
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
        return false;
    }

    function handleDragEnter(e) {
        // Add a visual cue when an item is dragged over another
        this.classList.add('over');
    }

    function handleDragLeave(e) {
        // Remove the visual cue when the dragged item leaves another item
        this.classList.remove('over');
    }

    function handleDrop(e) {
        // Prevent the default action and stop propagation
        e.stopPropagation();
        e.preventDefault();

        // Check if the item being dragged is dropped on a different item
        if (dragSrcEl !== this) {
            // Remove the dragged element from its original position
            dragSrcEl.parentNode.removeChild(dragSrcEl);
            // Insert the dragged element before the element it was dropped on
            this.insertAdjacentHTML('beforebegin', e.dataTransfer.getData('text/html'));
            // Update the newly inserted element with event listeners
            const dropElem = this.previousSibling;
            addDnDEvents(dropElem);

            // Update the order in localStorage
            updateOrder();
        }

        return false;
    }

    function handleDragEnd(e) {
        // Reset the opacity and remove 'over' class from all items
        this.style.opacity = '1';
        items.forEach(item => item.classList.remove('over'));
    }

    function addDnDEvents(elem) {
        // Attach all the necessary event listeners for drag and drop to an element
        elem.addEventListener('dragstart', handleDragStart, false);
        elem.addEventListener('dragenter', handleDragEnter, false);
        elem.addEventListener('dragover', handleDragOver, false);
        elem.addEventListener('dragleave', handleDragLeave, false);
        elem.addEventListener('drop', handleDrop, false);
        elem.addEventListener('dragend', handleDragEnd, false);
    }

    // Initial setup
    let dragSrcEl = null;
    loadOrder();
    let items = document.querySelectorAll('.ranking-list .box');
    items.forEach(addDnDEvents);
});
