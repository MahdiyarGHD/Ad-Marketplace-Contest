import { useEffect } from "react";
import Lottie from "../components/Lottie";
import { backButton, mainButton, openTelegramLink } from "@tma.js/sdk-react";
import "./AddChannel.scss";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router-dom";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";

function AddChannel() {
	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/my-channels");
	};

	const onSelectChannel = () => {
		openTelegramLink(
			`https://t.me/${import.meta.env.VITE_BOT_USERNAME}?startchannel=true&admin=invite_users+promote_members`,
		);
	};

	useEffect(() => {
		mainButton.setText("Select Channel");
		mainButton.onClick(onSelectChannel);
		mainButton.show();

		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			mainButton.hide();

			mainButton.offClick(onSelectChannel);

			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	return (
		<div className="AddChannel">
			<PageHeader>
				<PageHeaderTitle> </PageHeaderTitle>
			</PageHeader>

			<div className="Placeholder">
				<div className="Emoji">
					<Lottie file="bubble" size={120} />
				</div>
				<h2 className="Title">Add Your Channel</h2>
				<div className="Instructions">
					<p className="InstructionText">
						1. Make sure you are an admin of the channel you want to add.
					</p>
					<p className="InstructionText">
						2. Select the channel from the list.
					</p>
					<p className="InstructionText">3. Add this bot to your channel.</p>
					<p className="InstructionText"> - Tap "Administrators".</p>
					<p className="InstructionText">
						{" "}
						- Tap "Add Admin" and search for our Bot.
					</p>
					<p className="InstructionText">
						{" "}
						- Grant the necessary permissions and save.
					</p>
					<p className="InstructionText">
						4. You're all set! Start managing your channel ads.
					</p>
				</div>
			</div>
		</div>
	);
}

export default AddChannel;
