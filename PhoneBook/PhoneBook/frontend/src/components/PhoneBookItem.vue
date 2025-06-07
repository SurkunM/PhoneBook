<template>
    <tr>
        <td>
            <v-checkbox class="mt-5" v-model="isChecked" @change="switchCheckbox"></v-checkbox>
        </td>

        <td>{{ contact.index }}</td>

        <td>{{ contact.lastName }}</td>

        <td>{{ contact.firstName }}</td>

        <td>{{ contact.phone }}</td>

        <td>
            <div class="d-flex justify-center">
                <v-btn small color="info" @click="$emit('edit', contact)" class="me-2">Редактировать</v-btn>
                <v-btn small color="error" @click="$emit('delete', contact)">Удалить</v-btn>
            </div>
        </td>
    </tr>
</template>

<script>

    export default {
        props: {
            contact: {
                type: Object,
                required: true
            }
        },

        data() {
            return {
                isChecked: this.contact.isChecked
            };
        },

        watch: {
            'contact.isChecked'(value) {
                this.isChecked = value;
            }
        },

        methods: {
            switchCheckbox() {
                if (this.isChecked) {
                    this.$store.commit("addContactId", this.contact.id);
                } else {
                    this.$store.commit("removeContactId", this.contact.id);
                }
            }
        },

        emits: ["edit", "delete"]
    }
</script>