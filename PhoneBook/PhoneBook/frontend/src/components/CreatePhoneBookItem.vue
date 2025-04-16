<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-plus" size="small"></v-icon>
            Новый контакт
        </h2>
    </v-card-title>

    <form @submit.prevent="submitForm">
        <v-text-field v-model.trim="contact.firstName"
                      :error-messages="errors.firstName"
                      label="Имя">
        </v-text-field>

        <v-text-field v-model.trim="contact.lastName"
                      :error-messages="errors.lastName"
                      label="Фамилия">
        </v-text-field>

        <v-text-field v-model.trim="contact.phone"
                      :error-messages="errors.phone"
                      label="Телефон">
        </v-text-field>

        <v-btn class="me-4" type="submit">Сохранить</v-btn>

        <v-btn @click="resetForm">Очистить</v-btn>
    </form>
</template>

<script>
    export default {
        data() {
            return {
                id: 5,

                contact: {
                    firstName: "",
                    lastName: "",
                    phone: ""
                },
                errors: {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
            }
        },

        methods: {
            validateFirstName(firstName) {
                if (firstName?.length > 0) {
                    return true;
                }

                return "Заполните поле Имя";
            },

            validateLastName(lastName) {
                if (lastName?.length >= 2) {
                    return true;
                }

                return "Заполните поле Фамилия";
            },

            validatePhone(phone) {
                if (/^[0-9-]{7,}$/.test(phone)) {
                    return true;
                }

                return "Неверный формат для поля телефон";
            },

            submitForm() {
                this.errors.firstName = this.validateFirstName(this.contact.firstName);
                this.errors.lastName = this.validateLastName(this.contact.lastName);
                this.errors.phone = this.validatePhone(this.contact.phone);

                this.id++;

                const cratedContact = {
                    id: this.id,
                    firstName: this.contact.firstName,
                    lastName: this.contact.lastName,
                    phone: this.contact.phone,

                    isChecked: false,
                    isShown: true
                };

                this.$emit('create', cratedContact);
                //this.resetForm();
                alert("asd");
            },

            resetForm() {
                this.contact = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
                this.errors = {
                    firstName: "",
                    lastName: "",
                    phone: ""
                }
            }
        }
    }
</script>