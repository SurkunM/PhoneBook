<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-multiple" size="small"></v-icon>
            Контакты
        </h2>
    </v-card-title>

    <v-progress-linear v-if="isLoading"
                       indeterminate
                       color="primary"
                       height="4">
    </v-progress-linear>

    <v-alert type="success" variant="outlined" v-show="isShowSuccessAlert">
        <template v-slot:text>
            <span v-text="alertText"></span>
        </template>
    </v-alert>
    <v-alert type="error" variant="outlined" v-show="isShowErrorAlert">
        <template v-slot:text>
            <span v-text="alertText"></span>
        </template>
    </v-alert>

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
                      :items-per-page="contactsPerPage">

            <template v-slot:[`header.lastName`]="{ column }">
                <button @click="sortBy(column)">Фамилия</button>
            </template>

            <template v-slot:[`header.firstName`]="{ column }">
                <button @click="sortBy(column)">Имя</button>
            </template>

            <template v-slot:[`header.phone`]="{ column }">
                <button @click="sortBy(column)">Телефон</button>
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

            EditingModal,
            SingleDeleteModal,
            AllDeleteModal
        },

        data() {
            return {
                selectedContact: null,
                term: "",

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",

                headers: [
                    { value: "data-table-select" },
                    { value: "id", title: "№" },
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
                this.$store.dispatch("searchContacts", this.term);
            },

            cancelSearch() {
                this.term = "";
                this.$store.dispatch("searchContacts", this.term);
            },

            exportToExcel() {
                this.$store.dispatch("exportToExcel")
                    .then(() => {
                        this.showSuccessAlert("Контакт успешно выгружены в Excel.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка при выгрузке в Excel");                        
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
                        }
                        else if (error.response?.status === 409) {
                            this.$refs.contactEditingModal.setExistPhoneInvalid();
                        }
                    });
            },

            switchAllSelect() {
                this.$store.dispatch("switchAllSelect");
            },

            sortBy(column) {
                if (this.sortByColumn === column.value) {
                    this.sortDesc = !this.sortDesc;
                } else {
                    this.sortDesc = false;
                    this.sortByColumn = column.value;
                }

                this.$store.dispatch("sortByColumn", {
                    sortBy: this.sortByColumn,
                    isDesc: this.sortDesc
                });
            },

            switchPage(nextPage) {
                this.$store.dispatch("navigateToPage", nextPage);
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;

                setTimeout(() => {
                    this.alertText = "";
                    this.isShowSuccessAlert = false;
                }, 2000);
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;

                setTimeout(() => {
                    this.alertText = "";
                    this.isShowErrorAlert = false;
                }, 2000);
            }
        }
    };
</script>