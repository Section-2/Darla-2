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
        e.stopPropagation();
        e.preventDefault();

        if (dragSrcEl !== this) {
            // Remove the dragged element from its original position
            dragSrcEl.parentNode.removeChild(dragSrcEl);

            // Insert the dragged element before or after the target element
            // depending on whether it's being dragged up or down.
            let target = this;
            const dropHTML = e.dataTransfer.getData('text/html');
            if (this.nextSibling && dragSrcEl.nextSibling === this.nextSibling.nextSibling) {
                target = this.nextSibling;
                target.insertAdjacentHTML('afterend', dropHTML);
            } else {
                target.insertAdjacentHTML('beforebegin', dropHTML);
            }
            const dropElem = (this.nextSibling && dragSrcEl.nextSibling === this.nextSibling.nextSibling)
                ? target.nextSibling
                : this.previousSibling;

            addDnDEvents(dropElem);

            // Clear the dragged content from the source element
            e.dataTransfer.clearData();

            // Update the order in local storage after drop
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

    function resetOpacity() {
        const boxes = document.querySelectorAll('.ranking-list .box');
        boxes.forEach(box => {
            box.style.opacity = '';  // Reset opacity to default
            box.classList.remove('over');  // Remove the 'over' class to get rid of the dotted outline
        });
    }

    // Event listener for the "Save" button
    // const saveButton = document.getElementById('saveOrderButton');
    // saveButton.addEventListener('click', () => {
    //     resetOpacity();
    //     // Here you can also implement any other save logic if needed
    // })

    // Initial setup
    let dragSrcEl = null;
    loadOrder();
    let items = document.querySelectorAll('.ranking-list .box');
    items.forEach(addDnDEvents);

    document.addEventListener('submit', function(event){
         //Prevent the default form submission to update the inputs first
        if (event.target && event.target.id === 'rankingsForm') {
            event.preventDefault();

             //Your existing code to update the localStorage with the current order
            updateOrder();

             //Now, update the hidden inputs before submitting the form
            const itemOrder = JSON.parse(localStorage.getItem('itemOrder'));
            itemOrder.forEach(({id, position}) => {
                 //Find the hidden input for this team's rank and update its value
                document.querySelector(`#rankForTeam-${id}`).value = position + 1; // Assuming position is 0-indexed
            });

             //Now submit the form programmatically since we prevented the default action
            event.target.submit();
        }
    });
});
