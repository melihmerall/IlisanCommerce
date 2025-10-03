(function () {
    const managers = document.querySelectorAll('[data-variant-manager]');
    if (!managers.length) {
        return;
    }

    managers.forEach(initializeManager);

    function initializeManager(manager) {
        const tableBody = manager.querySelector('[data-variant-table-body]');
        const template = manager.querySelector('#variant-row-template');
        if (!tableBody || !template) {
            return;
        }

        let counter = parseInt(manager.dataset.variantCount || '0', 10);
        if (Number.isNaN(counter)) {
            counter = tableBody.querySelectorAll('.variant-row').length;
        }

        const counterInput = manager.querySelector('[data-variant-index]');
        const addButtons = manager.querySelectorAll('[data-variant-add]');

        addButtons.forEach(button => {
            button.addEventListener('click', event => {
                event.preventDefault();
                removeEmptyState();
                addRow();
            });
        });

        tableBody.querySelectorAll('.variant-row').forEach(row => {
            bindRow(row);
        });

        ensureEmptyState();
        refreshSort();

        function addRow() {
            const index = counter++;
            if (counterInput) {
                counterInput.value = counter;
            }

            const html = template.innerHTML.replace(/__index__/g, index);
            const holder = document.createElement('tbody');
            holder.innerHTML = html.trim();
            const row = holder.firstElementChild;
            row.dataset.variantIndex = index.toString();
            tableBody.appendChild(row);
            bindRow(row);
            refreshSort();
        }

        function bindRow(row) {
            const isExisting = row.dataset.existing === 'true';

            const defaultCheckbox = row.querySelector('.variant-default');
            const defaultHidden = row.querySelector('[data-variant-default-hidden]');
            if (defaultCheckbox && defaultHidden) {
                defaultHidden.value = defaultCheckbox.checked ? 'true' : 'false';
                defaultCheckbox.addEventListener('change', () => {
                    if (!defaultCheckbox.checked) {
                        defaultHidden.value = 'false';
                        return;
                    }

                    manager.querySelectorAll('.variant-default').forEach(cb => {
                        if (cb !== defaultCheckbox) {
                            cb.checked = false;
                        }
                    });

                    manager.querySelectorAll('[data-variant-default-hidden]').forEach(hidden => {
                        hidden.value = 'false';
                    });

                    defaultCheckbox.checked = true;
                    defaultHidden.value = 'true';
                });
            }

            const activeCheckbox = row.querySelector('.variant-active');
            const activeHidden = row.querySelector('[data-variant-active-hidden]');
            if (activeCheckbox && activeHidden) {
                activeHidden.value = activeCheckbox.checked ? 'true' : 'false';
                activeCheckbox.addEventListener('change', () => {
                    activeHidden.value = activeCheckbox.checked ? 'true' : 'false';
                });
            }

            const removeButton = row.querySelector('[data-variant-remove]');
            if (removeButton) {
                removeButton.addEventListener('click', () => {
                    const deletedField = row.querySelector('.variant-deleted');
                    if (deletedField) {
                        deletedField.value = 'true';
                    }

                    if (!isExisting) {
                        row.remove();
                    } else {
                        row.classList.add('variant-row--removed', 'd-none');
                    }

                    const activeHiddenField = row.querySelector('[data-variant-active-hidden]');
                    if (activeHiddenField) {
                        activeHiddenField.value = 'false';
                    }

                    ensureEmptyState();
                    refreshSort();
                });
            }
        }

        function refreshSort() {
            const activeRows = Array.from(tableBody.querySelectorAll('.variant-row'))
                .filter(row => !row.classList.contains('variant-row--removed'));

            activeRows.forEach((row, index) => {
                const sortInput = row.querySelector('.variant-sort');
                if (sortInput) {
                    sortInput.value = index.toString();
                }
            });

            ensureEmptyState();
        }

        function ensureEmptyState() {
            const activeRows = Array.from(tableBody.querySelectorAll('.variant-row'))
                .filter(row => !row.classList.contains('variant-row--removed'));
            const emptyRow = tableBody.querySelector('.variant-empty');

            if (!activeRows.length) {
                if (!emptyRow) {
                    const row = document.createElement('tr');
                    row.className = 'variant-empty';
                    row.innerHTML = '<td colspan="9" class="text-muted text-center">Henüz varyant eklenmedi.</td>';
                    tableBody.appendChild(row);
                }
            } else if (emptyRow) {
                emptyRow.remove();
            }
        }

        function removeEmptyState() {
            const emptyRow = tableBody.querySelector('.variant-empty');
            if (emptyRow) {
                emptyRow.remove();
            }
        }
    }
})();
