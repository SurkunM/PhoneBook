<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-multiple" size="small"></v-icon>
            Контакты
        </h2>
    </v-card-title>

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
                          single-line>
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
        <v-data-table :headers="headers"
                      :items="contacts"
                      :item-value="id">

            <template v-slot:[`header.data-table-select`]>
                <v-checkbox v-model="isAllSelect"
                            @change="switchAllSelect"
                            hide-details
                            class="ms-2">
                </v-checkbox>
            </template>

            <template v-slot:[`header.actions`]>
                <v-btn color="error"
                       text="Удалить все"
                       @click="showAllDeleteModal"
                       v-show="enabledButton">
                </v-btn>
            </template>

            <template v-slot:item="{item, index}">
                <phone-book-item :contact="item"
                                 :index="index"
                                 @delete="showSinglDeleteModal"
                                 @edit="showEditingModal">
                </phone-book-item>
            </template>
        </v-data-table>

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
                    { value: "lastName", title: "Фамилия", sortable: true },
                    { value: "firstName", title: "Имя", sortable: true },
                    { value: "phone", title: "Телефон" },
                    { value: "actions", title: "" }
                ]
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
            }
        },

        methods: {
            searchContacts() {
                this.$store.dispatch("searchContacts", this.term); //TODO: 3. Реализовать сортировку по клику headers
            },

            cancelSearch() {
                this.term = "",
                this.$store.dispatch("searchContacts", this.term);
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
                this.$store.dispatch("updateContat", contact)
                    .then(() => {
                        this.$refs.contactEditingModal.hide();
                        this.showSuccessAlert("Контакт успешно изменен.");
                    })
                    .catch(() => {//проверять поля
                        this.$refs.contactEditingModal.checkEditingFieldsIsvalid(contact);
                    });
            },

            switchAllSelect() {
                this.$store.dispatch("switchAllSelect");
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;

                setTimeout(() => {
                    this.alertText = "";
                    this.isShowSuccessAlert = false;
                }, 1500);
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;

                setTimeout(() => {
                    this.alertText = "";
                    this.isShowErrorAlert = false;
                }, 1500);
            }
        }
    };
</script>