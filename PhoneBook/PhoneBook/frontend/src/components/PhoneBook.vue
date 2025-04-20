<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-multiple" size="small"></v-icon>
            Контакты
        </h2>
    </v-card-title>
    <v-card flat>
        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Поиск"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line>
                <template v-slot:append-inner>
                    <v-btn icon
                           @click="search"
                           color="primary"
                           size="small">
                        <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-icon @click="term = ''"
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
                      :item-value="id"
                      :search="term">
            <!--чекбокс-->
            <template v-slot:[`header.data-table-select`]>
                <v-checkbox v-model="selectAll"
                            @change="toggleAllSelect"
                            hide-details
                            class="ms-2">
                </v-checkbox>
            </template>

            <template v-slot:[`header.actions`]>
                <v-btn color="error"
                       text="Удалить все"
                       @click="showAllDeleteModal">
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
                selectAll: false,
                headers: [
                    { value: "data-table-select", sortable: false },
                    { value: "id", title: "№" },
                    { value: "lastName", title: "Фамилия", sortable: true },
                    { value: "firstName", title: "Имя", sortable: true },
                    { value: "phone", title: "Телефон" },
                    { value: "actions", title: "", sortable: false }
                ]
            };
        },

        created() {
            this.$store.dispatch("loadContacts");
        },

        computed: {
            contacts() {
                return this.$store.state.contacts;
            }
        },

        methods: {
            toggleAllSelect() {
                this.$store.dispatch("toggleAllSelect", this.selectAll);
            },

            search() {
                alert("search");
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
                        this.$store.dispatch("loadContacts");
                        alert("Ошибка удаления в PhoneBook");
                    })
                    .finally(() => {
                        this.$refs.confirmSingleDeleteModal.hide();
                    });
            },

            deleteAllSelected() {
                this.contacts = this.contacts.filter(
                    contact => !this.selected.includes(contact)
                );
                this.selected = [];

                this.$refs.confirmAllDeleteModal.hide();
            },

            saveEditing(contact) {
                this.$store.dispatch("updateContat", contact)//TODO: Если не удалось изменить, то вернуть в lable старые значения контакта

                this.$refs.contactEditingModal.hide();
            },
        }
    };
</script>