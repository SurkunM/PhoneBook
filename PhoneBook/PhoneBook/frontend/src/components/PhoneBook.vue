<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-multiple" size="small"></v-icon>
            Контакты
        </h2>
    </v-card-title>

    <v-snackbar v-model="isShowSuccessAlert"
                :timeout="2000"
                color="success">
        {{alertText}}
    </v-snackbar>
    <v-snackbar v-model="isShowErrorAlert"
                :timeout="2000"
                color="error">
        {{alertText}}
    </v-snackbar>

    <v-card flat>
        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Поиск"
                          autocomplete="off"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line
                          @keyup.enter="searchContacts">
                <template v-slot:append-inner>
                    <v-btn icon
                           @click="searchContacts"
                           color="primary"
                           size="small">
                        <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-icon @click="cancelSearch"
                            style="cursor: pointer;"
                            size="x-large"
                            class="ms-1 me-2">
                        mdi-close-circle
                    </v-icon>
                </template>
            </v-text-field>
        </template>

        <div class="d-flex justify-end">
            <v-btn @click="exportToExcel"
                   color="primary"
                   size="small"
                   class="me-4">
                <v-icon>mdi-format-vertical-align-bottom</v-icon>Скачать Excel
            </v-btn>
        </div>

        <v-data-table :headers="headers"
                      :items="contacts"
                      :item-value="id"
                      hide-default-footer
                      :loading="isLoading"
                      :items-per-page="contactsPerPage"
                      no-data-text="Список контактов пуст">

            <template v-slot:[`header.lastName`]="{ column }">
                <button @click="sortBy(column)">Фамилия</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.firstName`]="{ column }">
                <button @click="sortBy(column)">Имя</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.phone`]="{ column }">
                <button @click="sortBy(column)">Телефон</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.data-table-select`]>
                <v-checkbox v-model="isAllSelect"
                            @change="switchAllSelect"
                            hide-details
                            class="ms-2">
                </v-checkbox>
            </template>

            <template v-slot:[`header.actions`]>
                <div class="d-flex justify-center">
                    <v-btn color="error"
                           text="Удалить все"
                           @click="showAllDeleteModal"
                           v-show="enabledButton">
                    </v-btn>
                </div>
            </template>

            <template v-slot:item="{ item }">
                <phone-book-item :contact="item"
                                 @delete="showSinglDeleteModal"
                                 @edit="showEditingModal">
                </phone-book-item>
            </template>
        </v-data-table>

        <v-pagination v-model="currentPage"
                      :length="pagesCount"
                      @update:modelValue="switchPage"
                      circle
                      color="primary">
        </v-pagination>

        <template>
            <single-delete-modal ref="confirmSingleDeleteModal" @delete="deleteContact"></single-delete-modal>
        </template>

        <template>
            <editing-modal ref="contactEditingModal" @save="saveEditing"></editing-modal>
        </template>

        <template>
            <all-delete-modal ref="confirmAllDeleteModal" @delete="deleteAllSelected"></all-delete-modal>
        </template>
    </v-card>
</template>

<script>
    import PhoneBookItem from "./PhoneBookItem.vue";

    import SingleDeleteModal from "./SingleDeleteModal.vue";
    import AllDeleteModal from "./AllDeleteModal.vue";
    import EditingModal from "./EditingModal.vue";

    export default {
        components: {
            PhoneBookItem,

            AllDeleteModal,
            SingleDeleteModal,

            EditingModal
        },

        data() {
            return {
                selectedContact: null,

                isSearchMode: false,
                term: "",

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",

                headers: [
                    { value: "data-table-select" },
                    { value: "index", title: "№" },
                    { value: "lastName", title: "Фамилия" },
                    { value: "firstName", title: "Имя" },
                    { value: "phone", title: "Телефон" },
                    { value: "actions", title: "" }
                ],

                currentPage: 1,

                sortByColumn: "lastName",
                sortDesc: false
            };
        },

        created() {
            this.$store.dispatch("loadContacts")
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить контакты.");
                });
        },

        computed: {
            contacts() {
                return this.$store.getters.contacts;
            },

            isAllSelect() {
                return this.$store.getters.isAllSelect;
            },

            enabledButton() {
                return this.$store.getters.hasSelected;
            },

            contactsPerPage() {
                return this.$store.getters.pageSize;
            },

            pagesCount() {
                return Math.ceil(this.$store.getters.contactsCount / this.contactsPerPage);
            },

            isLoading() {
                return this.$store.getters.isLoading;
            }
        },

        methods: {
            searchContacts() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);
                this.$store.commit("switchAllCheckbox", true);

                this.$store.dispatch("loadContacts")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить контакты.");
                    });

                this.isSearchMode = true;
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);
                this.$store.commit("switchAllCheckbox", true);

                this.$store.dispatch("loadContacts")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить контакты.");
                    });

                this.isSearchMode = false;
            },

            exportToExcel() {
                this.$store.dispatch("exportToExcel")
                    .then(() => {
                        this.showSuccessAlert("Контакы успешно выгружены в Excel.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось выгрузить контакты в excel файл.");
                    });
            },

            showAllDeleteModal() {
                this.$refs.confirmAllDeleteModal.show();
            },

            showSinglDeleteModal(contact) {
                this.selectedContact = contact;
                this.$refs.confirmSingleDeleteModal.show();
            },

            showEditingModal(contact) {
                this.selectedContact = contact;
                this.$refs.contactEditingModal.show(this.selectedContact);
            },

            deleteContact() {
                this.$store.dispatch("deleteContact", this.selectedContact.id)
                    .then(() => {
                        this.showSuccessAlert("Контакт успешно удален.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось удалить контакт.");
                    })
                    .finally(() => {
                        this.$refs.confirmSingleDeleteModal.hide();
                    });
            },

            deleteAllSelected() {
                this.$store.dispatch("deleteAllSelectedContacts")
                    .then(() => {
                        this.showSuccessAlert("Выбранные контакты успешно удалены.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось удалить выбранные контакты.");
                    })
                    .finally(() => {
                        this.$refs.confirmSingleDeleteModal.hide();
                    });

                this.$refs.confirmAllDeleteModal.hide();
            },

            saveEditing(contact) {
                this.$store.dispatch("updateContact", contact)
                    .then(() => {
                        this.$refs.contactEditingModal.hide();
                        this.showSuccessAlert("Контакт успешно изменен.");
                    })
                    .catch(error => {
                        if (error.response?.status === 400) {
                            this.$refs.contactEditingModal.checkEditingFieldsIsvalid(contact);
                        } else if (error.response?.status === 409) {
                            this.$refs.contactEditingModal.setExistPhoneInvalid();
                        }
                    });
            },

            switchAllSelect() {
                this.$store.commit("switchAllCheckbox", this.isAllSelect);
            },

            sortBy(column) {
                if (this.sortByColumn === column.value) {
                    this.sortDesc = !this.sortDesc;
                } else {
                    this.sortDesc = false;
                    this.sortByColumn = column.value;
                }

                this.$store.commit("setSortingParameters", {
                    sortBy: this.sortByColumn,
                    isDesc: this.sortDesc
                })

                this.$store.dispatch("loadContacts")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить контакты.");
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadContacts")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить контакты.");
                    });
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;
            }
        }
    };
</script>