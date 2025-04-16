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
        <v-data-table v-model="selected"
                      :headers="headers"
                      :items="contacts"
                      :item-value="id"
                      :search="term">

            <template v-slot:[`header.data-table-select`]>
                <v-checkbox hide-details class="ms-2"></v-checkbox>
            </template>

            <template v-slot:[`header.actions`]>
                <v-btn color="error"
                       text="Удалить все"
                       :disabled="selected.length === 12"
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
            <all-delete-modal ref="confirmAllDeleteModal" @delete="deleteAllSelected"></all-delete-modal>
        </template>

        <template>
            <editing-modal ref="contactEditingModal" @save="saveEditing"></editing-modal>
        </template>

        <template>
            <create-phone-book-item @create="addContact"></create-phone-book-item>
        </template>
    </v-card>
</template>

<script>
    import PhoneBookItem from "./PhoneBookItem.vue";
    import CreatePhoneBookItem from "./CreatePhoneBookItem.vue";

    import SingleDeleteModal from "./SingleDeleteModal.vue";
    import AllDeleteModal from "./AllDeleteModal.vue";
    import EditingModal from "./EditingModal.vue"

    export default {
        components: {
            CreatePhoneBookItem,
            PhoneBookItem,
            EditingModal,

            SingleDeleteModal,
            AllDeleteModal
        },

        data() {
            return {
                selectedContact: null,
                selected: [],
                term: "",

                headers: [
                    { value: "data-table-select", sortable: false },
                    { value: "id", title: "№" },
                    { value: "lastName", title: "Фамилия" },
                    { value: "firstName", title: "Имя" },
                    { value: "phone", title: "Телефон" },
                    { value: "actions", title: "", sortable: false }
                ],

                contacts: [
                    {
                        id: 1,
                        lastName: 'Иванов',
                        firstName: "Иван",
                        phone: "+7 (987) 654-32-10"
                    },
                    {
                        id: 2,
                        lastName: 'Петрова',
                        firstName: "Анна",
                        phone: "+7 (987) 654-32-11"
                    },
                    {
                        id: 3,
                        lastName: 'Смирнова',
                        firstName: "Елена",
                        phone: "+7 (987) 654-32-12"
                    }
                ]
            };
        },

        methods: {
            search() {
                alert("search");
            },

            addContact(contact) {
                this.contacts.push(contact);
                alert("Create");
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

            deleteAllSelected() {
                this.contacts = this.contacts.filter(
                    contact => !this.selected.includes(contact)
                );
                this.selected = [];

                this.$refs.confirmAllDeleteModal.hide();
            },

            deleteContact() {
                console.log('Контакт удалён!');
                this.$refs.confirmSingleDeleteModal.hide();
            },

            saveEditing(contact) {
                const index = this.contacts.findIndex(c => c.id === contact.id);
                if (index !== -1) {
                    this.contacts[index] = contact;
                }

                this.$refs.contactEditingModal.hide();
            },
        }
    };
</script>