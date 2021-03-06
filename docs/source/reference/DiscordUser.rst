Reference for ``DiscordUser``
=============================

This class represents a Discord user, without any guild affiliations.

If you want to get a user associated to a guild, check out :doc:`DiscordGuild </reference/DiscordGuild>` and :doc:`DiscordMember </reference/DiscordMember>`.

Members
-------

.. attribute:: Id

	This user's ID.

.. attribute:: Username

	This user's username, without a discriminator. For example, given ``b1nzy#1337``, this property returns ``b1nzy``.

.. attribute:: Discriminator

	This user's discriminator. For example, given ``b1nzy#1337``, this property returns ``1337``.

.. attribute:: AvatarHash

	Hash of this user's avatar image.

.. attribute:: AvatarUrl

	URL of this user's avatar.

.. attribute:: DefaultAvatarUrl

	URL of default avatar for this user.

.. attribute:: IsBot

	Whether or not this user is a bot.

.. attribute:: MfaEnabled

	Whether or not this user has mutli-factor authentication enabled. This value can be ``null``.

.. attribute:: Verified

	Whether or not this user has verified their email. This value can be ``null``.

.. attribute:: Email

	This user's email address. This value can be ``null``.

.. attribute:: Mention

	This user's mention string.

.. attribute:: Presence

	This user's presence and status. Instance of :doc:`DiscordPresence </reference/entities/DiscordPresence>`.